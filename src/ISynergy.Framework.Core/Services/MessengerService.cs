using ISynergy.Framework.Core.Abstractions.Events;
using ISynergy.Framework.Core.Abstractions.Services;
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
/// <para>
/// Recipient storage is delegated to <see cref="MessengerRecipientRegistry"/>; this type owns the
/// public register/send/unregister surface and message dispatch.
/// </para>
/// </remarks>
public sealed class MessengerService : IMessengerService
{
    private readonly ILogger<MessengerService> _logger;
    private readonly MessengerRecipientRegistry _registry = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MessengerService"/> class.
    /// </summary>
    /// <param name="logger">Optional logger for exception handling. If not provided, exceptions will be silently handled.</param>
    public MessengerService(ILogger<MessengerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage. The action parameter will be executed
    /// when a corresponding message is sent.
    /// <para>Registering a recipient does not create a hard reference to it, so if this recipient is
    /// deleted, no memory leak is caused.</para>
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="action">The action that will be executed when a message of type TMessage is sent.
    /// IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference,
    /// which might cause a memory leak. Only set this to true if the action is using closures.</param>
    public void Register<TMessage>(
        object recipient,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, null, false, action, keepTargetAlive);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage on a messaging channel token.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel, or <see langword="null"/>.</param>
    /// <param name="action">The action that will be executed when a message of type TMessage is sent.
    /// IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference,
    /// which might cause a memory leak. Only set this to true if the action is using closures.</param>
    public void Register<TMessage>(
        object recipient,
        object token,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, token, false, action, keepTargetAlive);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage, optionally also receiving derived/interface messages.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from TMessage (or implementing
    /// TMessage when it is an interface) are also transmitted to the recipient.</param>
    /// <param name="action">The action that will be executed when a message of type TMessage is sent.
    /// IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference,
    /// which might cause a memory leak. Only set this to true if the action is using closures.</param>
    public void Register<TMessage>(
        object recipient,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        Register(recipient, null, receiveDerivedMessagesToo, action, keepTargetAlive);
    }

    /// <summary>
    /// Registers a recipient for a type of message TMessage.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that the recipient registers for.</typeparam>
    /// <param name="recipient">The recipient that will receive the messages.</param>
    /// <param name="token">A token for a messaging channel, or <see langword="null"/>.</param>
    /// <param name="receiveDerivedMessagesToo">If true, message types deriving from TMessage (or implementing
    /// TMessage when it is an interface) are also transmitted to the recipient.</param>
    /// <param name="action">The action that will be executed when a message of type TMessage is sent.
    /// IMPORTANT: If the action causes a closure, you must set keepTargetAlive to true to avoid side effects.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will be kept as a hard reference,
    /// which might cause a memory leak. Only set this to true if the action is using closures.</param>
    public void Register<TMessage>(
        object recipient,
        object? token,
        bool receiveDerivedMessagesToo,
        Action<TMessage> action,
        bool keepTargetAlive = false)
    {
        _registry.Register(recipient, token, receiveDerivedMessagesToo, action, keepTargetAlive);
        Cleanup();
    }

    /// <summary>
    /// Sends a message to registered recipients.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public void Send<TMessage>(TMessage message)
    {
        SendToTargetOrType(message, null, null);
    }

    /// <summary>
    /// Sends a message to registered recipients of a specific target type.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <typeparam name="TTarget">The type of recipients that will receive the message.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    public void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof(TTarget), null);
    }

    /// <summary>
    /// Sends a message to registered recipients on a specific channel token.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="token">A token for a messaging channel.</param>
    public void Send<TMessage>(TMessage message, object token)
    {
        SendToTargetOrType(message, null, token);
    }

    /// <summary>
    /// Unregisters a messager recipient completely. After this method is executed, the recipient
    /// will not receive any messages anymore.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    public void Unregister(object recipient)
    {
        _registry.Unregister(recipient);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages only.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants to unregister from.</typeparam>
    public void Unregister<TMessage>(object recipient)
    {
        Unregister<TMessage>(recipient, null, null);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages and a given token.
    /// </summary>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <typeparam name="TMessage">The type of messages that the recipient wants to unregister from.</typeparam>
    public void Unregister<TMessage>(object recipient, object token)
    {
        Unregister<TMessage>(recipient, token, null);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages and a given action.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for the recipient and for the message type TMessage.</param>
    public void Unregister<TMessage>(object recipient, Action<TMessage> action)
    {
        Unregister(recipient, null, action);
    }

    /// <summary>
    /// Unregisters a message recipient for a given type of messages, action and token.
    /// </summary>
    /// <typeparam name="TMessage">The type of messages that the recipient wants to unregister from.</typeparam>
    /// <param name="recipient">The recipient that must be unregistered.</param>
    /// <param name="token">The token for which the recipient must be unregistered.</param>
    /// <param name="action">The action that must be unregistered for the recipient and for the message type TMessage.</param>
    public void Unregister<TMessage>(object recipient, object? token, Action<TMessage>? action)
    {
        _registry.Unregister(recipient, token, action);
        Cleanup();
    }

    /// <summary>
    /// Sends a message to the recipients registered for its type, optionally narrowed to a target type
    /// and/or a channel token.
    /// </summary>
    /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
    /// <param name="message">The message to send to registered recipients.</param>
    /// <param name="targetType">The type of recipients that will receive the message, or <see langword="null"/> for any.</param>
    /// <param name="token">A token for a messaging channel, or <see langword="null"/>.</param>
    public void SendToTargetOrType<TMessage>( // NOSONAR
        TMessage message,
        Type? targetType,
        object? token)
    {
        if (message is null)
            return;

        var messageType = typeof(TMessage);

        // Handle subclass/interface recipients
        if (_registry.HasSubclassRecipients)
        {
            foreach (var type in _registry.GetSubclassIndexKeys())
            {
                if (messageType == type
                    || messageType.IsSubclassOf(type)
                    || type.IsAssignableFrom(messageType))
                {
                    var list = _registry.GetSubclassRecipients(type);
                    if (list is not null)
                    {
                        SendToList(message, list, targetType, token);
                    }
                }
            }
        }

        // Handle strict type recipients
        if (_registry.HasStrictRecipients)
        {
            var list = _registry.GetStrictRecipients(messageType);
            if (list is not null)
            {
                SendToList(message, list, targetType, token);
            }
        }

        Cleanup();
    }

    /// <summary>
    /// Trims dead recipients from both indexes.
    /// </summary>
    private void Cleanup() => _registry.Cleanup();

    /// <summary>
    /// Invokes every live, matching recipient action in <paramref name="list"/>.
    /// </summary>
    private void SendToList<TMessage>(
        TMessage message,
        List<WeakActionAndToken> list,
        Type? targetType,
        object? token)
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
                    || targetType.IsInstanceOfType(item.Action.Target))
                && ((item.Token is null && token is null)
                    || item.Token is not null && item.Token.Equals(token))) // NOSONAR - item.Token can be null or non-null; both branches are reachable
            {
                try
                {
                    executeAction.ExecuteWithObject(message!);
                }
                catch (Exception ex)
                {
                    // Log exceptions to prevent one failing recipient from affecting others
                    // This allows debugging while maintaining message delivery to other recipients
                    _logger?.LogWarning(ex,
                        "Exception occurred while executing message handler. MessageType: {MessageType}, RecipientType: {RecipientType}",
                        message.GetType().Name,
                        item.Action?.Target?.GetType().Name ?? "Unknown");
                }
            }
        }
    }
}
