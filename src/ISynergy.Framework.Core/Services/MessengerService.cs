using ISynergy.Framework.Core.Abstractions.Events;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// A class providing a reference implementation for the <see cref="IMessengerService"/> interface.
/// </summary>
/// <remarks>
/// <para>
/// This <see cref="IMessengerService"/> implementation uses weak references to track the registered
/// recipients, so it is not necessary to manually unregister them when they're no longer needed.
/// </para>
/// <para>
/// The <see cref="MessengerService"/> type will automatically perform internal trimming when
/// full GC collections are invoked, so calling <see cref="Cleanup"/> manually is not necessary to
/// ensure that on average the internal data structures are as trimmed and compact as possible.
/// </para>
/// </remarks>
public sealed class MessengerService : IMessengerService
{
    private readonly object _recipientsLock = new object();
    private readonly object _cleanupLock = new object();
    private readonly ILogger<MessengerService>? _logger;

    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsOfSubclassesAction;
    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsStrictAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessengerService"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for exception handling. If not provided, exceptions will be silently handled.</param>
    public MessengerService(ILogger<MessengerService>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage. The action
    /// parameter will be executed when a corresponding message is sent.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.
    /// </param>
    public void Register<TMessage>(
        object recipient,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, null, false, action, keepTargetAlive);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding 
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// <para>However if you use closures and set keepTargetAlive to true, you might
    /// cause a memory leak if you don't call <see cref="Unregister"/> when you are cleaning up.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.
    /// </param>
    public void Register<TMessage>(
        object recipient,
        object token,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, token, false, action, keepTargetAlive);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding 
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
    /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.
    /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
    /// transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.</para>
    /// </param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.
    /// </param>
    public void Register<TMessage>(
        object recipient,
        object? token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        lock (_recipientsLock)
        {
            var messageType = typeof(TMessage);

            Dictionary<Type, List<WeakActionAndToken>>? recipients;

            if (receiveDerivedMessagesToo)
            {
                _recipientsOfSubclassesAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                recipients = _recipientsOfSubclassesAction;
            }
            else
            {
                _recipientsStrictAction ??= new Dictionary<Type, List<WeakActionAndToken>>();
                recipients = _recipientsStrictAction;
            }

            if (!recipients.TryGetValue(messageType, out var list))
            {
                list = new List<WeakActionAndToken>();
                recipients.Add(messageType, list);
            }

            var weakAction = new WeakAction<TMessage>(recipient, action, keepTargetAlive);

            var item = new WeakActionAndToken
            {
                Action = weakAction,
                Token = token
            };

            list.Add(item);
        }

        Cleanup();
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// The action parameter will be executed when a corresponding 
    /// message is sent. See the receiveDerivedMessagesToo parameter
    /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
    /// messages implementing TMessage) can be received too.
    /// <para>Registering a recipient does not create a hard reference to it,
    /// so if this recipient is deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers
    /// for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
    /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.
    /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
    /// transmitted to the recipient. For example, if a SendOrderMessage
    /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
    /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
    /// and ExecuteOrderMessage to the recipient that registered.</para>
    /// </param>
    /// <param name="action">The action that will be executed when a message
    /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
    /// you must set keepTargetAlive to true to avoid side effects. </param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures.
    /// </param>
    public void Register<TMessage>(
        object recipient,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, null, receiveDerivedMessagesToo, action, keepTargetAlive);
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach all recipients that registered for this message type
    /// using one of the Register methods.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public void Send<TMessage>(TMessage message)
    {
        SendToTargetOrType(message, null, null);
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are
    /// of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <typeparam name="TTarget">The type of recipients that will receive
    /// the message. The message won't be sent to recipients of another type.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof(TTarget), null);
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are
    /// of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    public void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, null, token);
    }

    /// <summary>
    /// Unregisters a messager recipient completely. After this method
    /// is executed, the recipient will not receive any messages anymore.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public void Unregister(object recipient)
    {
        UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
        UnregisterFromLists(recipient, _recipientsStrictAction);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only. 
    /// After this method is executed, the recipient will not receive messages
    /// of type TMessage anymore, but will still receive other message types (if it
    /// registered for them previously).
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    public void Unregister<TMessage>(object recipient)
    {
        Unregister<TMessage>(recipient, null, null);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only and for a given token. 
    /// After this method is executed, the recipient will not receive messages
    /// of type TMessage anymore with the given token, but will still receive other message types
    /// or messages with other tokens (if it registered for them previously).
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    public void Unregister<TMessage>(object recipient, object token)
    {
        Unregister<TMessage>(recipient, token, null);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages and for
    /// a given action. Other message types will still be transmitted to the
    /// recipient (if it registered for them previously). Other actions that have
    /// been registered for the message type TMessage and for the given recipient will
    /// also remain available.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for
    /// the recipient and for the message type TMessage.</param>
    public void Unregister<TMessage>(object recipient, Action<TMessage> action)
    {
        Unregister(recipient, null, action);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages, for
    /// a given action and a given token. Other message types will still be transmitted to the
    /// recipient (if it registered for them previously). Other actions that have
    /// been registered for the message type TMessage, for the given recipient and other tokens
    /// will also remain available.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants
    /// to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for
    /// the recipient and for the message type TMessage.</param>
    public void Unregister<TMessage>(object recipient, object? token, Action<TMessage>? action)
    {
        UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
        UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
        Cleanup();
    }

    /// <summary>
    /// Cleans up all the lists of recipients.
    /// Since recipients are stored as WeakReferences, 
    /// recipients that are cleared from memory will automatically be removed from the lists.
    /// This method is typically called after a few Unregister operations 
    /// (but not after each Unregister, for performance reasons) 
    /// and never called after a Register operation.
    /// </summary>
    private void Cleanup()
    {
        lock (_cleanupLock)
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
        }
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="targetType">The type of recipients that will receive
    /// the message. The message won't be sent to recipients of another type.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    public void SendToTargetOrType<TMessage>(
        TMessage message,
        Type? targetType,
        object? token)
    {
        if (message is null)
            return;

        var messageType = typeof(TMessage);

        // Handle subclass recipients
        if (_recipientsOfSubclassesAction is not null)
        {
            List<Type> typesToProcess;
            lock (_recipientsLock)
            {
                // Create a simple copy of the keys - removed unnecessary Take() operation
                typesToProcess = _recipientsOfSubclassesAction.Keys.ToList();
            }

            foreach (var type in typesToProcess)
            {
                if (messageType == type
                    || messageType.IsSubclassOf(type)
                    || type.IsAssignableFrom(messageType))
                {
                    List<WeakActionAndToken>? list = null;

                    lock (_recipientsLock)
                    {
                        if (_recipientsOfSubclassesAction.TryGetValue(type, out var sourceList) && sourceList.Count > 0)
                        {
                            // Create a safe copy of the list to avoid modification during iteration
                            list = sourceList.ToList();
                        }
                    }

                    if (list is not null)
                    {
                        SendToList(message, list, messageType, targetType, token, _logger);
                    }
                }
            }
        }

        // Handle strict type recipients
        if (_recipientsStrictAction is not null)
        {
            List<WeakActionAndToken>? list = null;

            lock (_recipientsLock)
            {
                if (_recipientsStrictAction.TryGetValue(messageType, out var sourceList) && sourceList.Count > 0)
                {
                    // Create a safe copy of the list to avoid modification during iteration
                    list = sourceList.ToList();
                }
            }

            if (list is not null)
            {
                SendToList(message, list, messageType, targetType, token, _logger);
            }
        }

        Cleanup();
    }

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach only recipients that registered for this message type
    /// using one of the Register methods, and that are of the targetType.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="list">The list of recipients that will receive
    /// the message.</param>
    /// <param name="messageType">The type of message that will be sent.</param>
    /// <param name="targetType">The type of recipients that will receive
    /// the message. The message won't be sent to recipients of another type.</param>
    /// <param name="token">A token for a messaging channel. If a recipient registers
    /// using a token, and a sender sends a message using the same token, then this
    /// message will be delivered to the recipient. Other recipients who did not
    /// use a token when registering (or who used a different token) will not
    /// get the message. Similarly, messages sent without any token, or with a different
    /// token, will not be delivered to that recipient.</param>
    private static void SendToList<TMessage>(
        TMessage message,
        List<WeakActionAndToken> list,
        Type messageType,
        Type? targetType,
        object? token,
        ILogger<MessengerService>? logger)
    {
        if (message is null)
            return;

        foreach (var item in list)
        {
            // Enhanced null safety and exception handling
            if (item?.Action is IExecuteWithObject executeAction
                && item.Action.IsAlive
                && item.Action.Target is not null
                && (targetType is null
                    || item.Action.Target.GetType() == targetType
                    || targetType.IsAssignableFrom(item.Action.Target.GetType()))
                && ((item.Token is null && token is null)
                    || item.Token is not null && item.Token.Equals(token)))
            {
                try
                {
                    executeAction.ExecuteWithObject(message);
                }
                catch (Exception ex)
                {
                    // Log exceptions to prevent one failing recipient from affecting others
                    // This allows debugging while maintaining message delivery to other recipients
                    logger?.LogWarning(ex, 
                        "Exception occurred while executing message handler. MessageType: {MessageType}, RecipientType: {RecipientType}",
                        message?.GetType().Name ?? "Unknown",
                        item.Action?.Target?.GetType().Name ?? "Unknown");
                }
            }
        }
    }

    private void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (recipient is null || lists is null || lists.Count == 0)
        {
            return;
        }

        lock (lists)
        {
            foreach (var messageType in lists.Keys)
            {
                var items = lists[messageType];
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item?.Action is not null && item.Action.Target == recipient)
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }
    }

    private static void UnregisterFromLists<TMessage>(
        object recipient,
        object? token,
        Action<TMessage>? action,
        Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        var messageType = typeof(TMessage);

        if (recipient is null
            || lists is null
            || lists.Count == 0
            || !lists.TryGetValue(messageType, out var items))
        {
            return;
        }

        lock (lists)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item?.Action is not null
                    && item.Action.Target == recipient
                    && (action is null || item.Action.MethodName == action.Method.Name)
                    && (token is null || item.Token is not null && item.Token.Equals(token)))
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    private static void CleanupList(Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (lists is null)
        {
            return;
        }

        lock (lists)
        {
            var typesToRemove = new List<Type>();

            foreach (var kvp in lists)
            {
                var messageType = kvp.Key;
                var recipients = kvp.Value;

                // Remove dead references in reverse order to avoid index issues
                for (int i = recipients.Count - 1; i >= 0; i--)
                {
                    var item = recipients[i];
                    if (item?.Action is null || !item.Action.IsAlive)
                    {
                        recipients.RemoveAt(i);
                    }
                }

                if (recipients.Count == 0)
                {
                    typesToRemove.Add(messageType);
                }
            }

            foreach (var key in typesToRemove)
            {
                lists.Remove(key);
            }
        }
    }

    private class WeakActionAndToken
    {
        public WeakAction? Action { get; set; }
        public object? Token { get; set; }
    }

}