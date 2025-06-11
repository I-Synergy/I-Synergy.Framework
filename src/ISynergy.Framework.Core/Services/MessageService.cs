using ISynergy.Framework.Core.Abstractions.Events;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Services;

/// <summary>
/// The Messenger is a class allowing objects to exchange messages.
/// </summary>
public class MessageService : IMessageService
{
    private readonly object _registerLock = new object();
    private readonly object _recipientsOfSubclassesActionLock = new object();
    private readonly object _recipientsStrictActionLock = new object();
    private readonly object _recipientsLock = new object();
    private readonly object _listLock = new object();

    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsOfSubclassesAction;
    private Dictionary<Type, List<WeakActionAndToken>>? _recipientsStrictAction;

    private static readonly object _creationLock = new object();
    private static IMessageService? _defaultInstance;

    /// <summary>
    /// Gets the Messenger's default instance, allowing
    /// to register and send messages in a static manner.
    /// </summary>
    public static IMessageService Default
    {
        get
        {
            if (_defaultInstance is null)
            {
                lock (_creationLock)
                {
                    if (_defaultInstance is null)
                    {
                        _defaultInstance = new MessageService();
                    }
                }
            }

            return _defaultInstance;
        }
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
    public virtual void Register<TMessage>(
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
    public virtual void Register<TMessage>(
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
    public virtual void Register<TMessage>(
        object recipient,
        object? token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        lock (_registerLock)
        {
            var messageType = typeof(TMessage);

            Dictionary<Type, List<WeakActionAndToken>>? recipients;

            if (receiveDerivedMessagesToo)
            {
                if (_recipientsOfSubclassesAction is null)
                {
                    _recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
                }

                recipients = _recipientsOfSubclassesAction;
            }
            else
            {
                if (_recipientsStrictAction is null)
                {
                    _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
                }

                recipients = _recipientsStrictAction;
            }

            lock (_recipientsLock)
            {
                List<WeakActionAndToken> list;

                if (!recipients.ContainsKey(messageType))
                {
                    list = new List<WeakActionAndToken>();
                    recipients.Add(messageType, list);
                }
                else
                {
                    list = recipients[messageType];
                }

                var weakAction = new WeakAction<TMessage>(recipient, action, keepTargetAlive);

                var item = new WeakActionAndToken
                {
                    Action = weakAction,
                    Token = token
                };

                list.Add(item);
            }
        }

        RequestCleanup();
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
    public virtual void Register<TMessage>(
        object recipient,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, null, receiveDerivedMessagesToo, action, keepTargetAlive);
    }

    private bool _isCleanupRegistered;

    /// <summary>
    /// Sends a message to registered recipients. The message will
    /// reach all recipients that registered for this message type
    /// using one of the Register methods.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public virtual void Send<TMessage>(TMessage message)
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
    public virtual void Send<TMessage, TTarget>(TMessage message)
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
    public virtual void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, null, token);
    }

    /// <summary>
    /// Unregisters a messager recipient completely. After this method
    /// is executed, the recipient will not receive any messages anymore.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public virtual void Unregister(object recipient)
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
    public virtual void Unregister<TMessage>(object recipient)
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
    public virtual void Unregister<TMessage>(object recipient, object token)
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
    public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
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
    public virtual void Unregister<TMessage>(object recipient, object? token, Action<TMessage>? action)
    {
        UnregisterFromLists(recipient, token, action, _recipientsStrictAction);
        UnregisterFromLists(recipient, token, action, _recipientsOfSubclassesAction);
        RequestCleanup();
    }

    /// <summary>
    /// Requests a cleanup of the Messenger's lists.
    /// </summary>
    public void RequestCleanup()
    {
        if (_isCleanupRegistered)
        {
            return;
        }

        _isCleanupRegistered = true;

        Task.Run(() =>
        {
            Cleanup();
            _isCleanupRegistered = false;
        });
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
        CleanupList(_recipientsOfSubclassesAction);
        CleanupList(_recipientsStrictAction);
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
    public virtual void SendToTargetOrType<TMessage>(
        TMessage message,
        Type? targetType,
        object? token)
    {
        var messageType = typeof(TMessage);

        if (_recipientsOfSubclassesAction is not null)
        {
            var listClone = _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count).ToList();

            foreach (var type in listClone.EnsureNotNull())
            {
                List<WeakActionAndToken>? list = null;

                if (messageType == type
                    || messageType.IsSubclassOf(type)
                    || type.IsAssignableFrom(messageType))
                {
                    lock (_recipientsOfSubclassesActionLock)
                    {
                        if (_recipientsOfSubclassesAction.ContainsKey(type) && _recipientsOfSubclassesAction[type].Count > 0)
                        {
                            // Create a safe copy of the list to avoid index out of range exceptions
                            list = _recipientsOfSubclassesAction[type].ToList();
                        }
                    }

                    SendToList(message, list, messageType, targetType, token);
                }
            }
        }

        if (_recipientsStrictAction is not null)
        {
            List<WeakActionAndToken>? list = null;

            lock (_recipientsStrictActionLock)
            {
                if (_recipientsStrictAction.ContainsKey(messageType) && _recipientsStrictAction[messageType].Count > 0)
                {
                    // Create a safe copy of the list to avoid index out of range exceptions
                    list = _recipientsStrictAction[messageType].ToList();
                }
            }

            SendToList(message, list, messageType, targetType, token);
        }

        RequestCleanup();
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
        List<WeakActionAndToken>? list,
        Type messageType,
        Type? targetType,
        object? token)
    {
        if (message is null)
            return;

        foreach (var item in list.EnsureNotNull())
        {
            if (item.Action is IExecuteWithObject executeAction
                && item.Action.IsAlive
                && item.Action.Target is not null
                && (targetType is null
                    || item.Action.Target.GetType() == targetType
                    || targetType.IsAssignableFrom(item.Action.Target.GetType()))
                && ((item.Token is null && token is null)
                    || item.Token is not null && item.Token.Equals(token)))
            {
                executeAction.ExecuteWithObject(message);
            }
        }
    }

    private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>>? lists)
    {
        if (recipient is null
            || lists is null
            || lists.Count == 0)
        {
            return;
        }

        lock (lists)
        {
            foreach (var messageType in lists.Keys.EnsureNotNull())
            {
                foreach (var item in lists[messageType].EnsureNotNull())
                {
                    if (item.Action is not null
                        && item.Action.Target == recipient)
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
            || !lists.ContainsKey(messageType))
        {
            return;
        }

        lock (lists)
        {
            foreach (var item in lists[messageType].EnsureNotNull())
            {
                if (item.Action is not null
                    && item.Action.Target == recipient
                    && (action is null
                        || item.Action.MethodName == action.Method.Name)
                    && (token is null
                        || item.Token is not null && item.Token.Equals(token)))
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
            var listsToRemove = new List<Type>();
            foreach (var messageType in lists.Keys)
            {
                var recipientsToRemove = lists[messageType]
                    .Where(item => item.Action is null || !item.Action.IsAlive)
                    .ToList();

                foreach (var recipient in recipientsToRemove.EnsureNotNull())
                {
                    lists[messageType].Remove(recipient);
                }

                if (lists[messageType].Count == 0)
                {
                    listsToRemove.Add(messageType);
                }
            }

            foreach (var key in listsToRemove)
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

    /// <summary>
    /// Provides a way to override the Messenger.Default instance with
    /// a custom instance, for example for unit testing purposes.
    /// </summary>
    /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
    public static void OverrideDefault(IMessageService newMessenger)
    {
        _defaultInstance = newMessenger;
    }

    /// <summary>
    /// Sets the Messenger's default (static) instance to null.
    /// </summary>
    public static void Reset()
    {
        _defaultInstance = null;
    }
}