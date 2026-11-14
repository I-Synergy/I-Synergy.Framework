using Azure.Core;
using Azure.Messaging.ServiceBus;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Azure.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ISynergy.Framework.MessageBus.Azure.Services.Queue;

/// <summary>
/// Message bus implementation on Azure Service Bus.
/// </summary>
/// <typeparam name="TQueueMessage">The type of the t queue message.</typeparam>
/// <typeparam name="TOption">The type of the t option.</typeparam>
internal class PublisherServiceBus<TQueueMessage, TOption> : IPublisherServiceBus<TQueueMessage>
    where TQueueMessage : class, IBaseMessage
    where TOption : class, IAzureQueueOption, new()
{
    private readonly TOption _option;
    private readonly JsonTypeInfo<TQueueMessage>? _jsonTypeInfo;
    private readonly TokenCredential? _credential;

    /// <summary>
    /// Constructor of service bus. Uses AOT-safe source-generated JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TQueueMessage"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>. Required for Native AOT publishing.
    /// </param>
    /// <param name="credential">
    /// Optional credential for identity-based authentication. When omitted, a credential is created
    /// from <see cref="IAzureQueueOption.ManagedIdentityClientId"/>.
    /// </param>
    public PublisherServiceBus(
        IOptions<TOption> options,
        JsonTypeInfo<TQueueMessage> jsonTypeInfo,
        TokenCredential? credential = null)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.QueueName);
        ValidateAuthenticationConfiguration();

        _jsonTypeInfo = jsonTypeInfo;
        _credential = credential;
    }

    /// <summary>
    /// Constructor of service bus. Uses reflection-based JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="credential">
    /// Optional credential for identity-based authentication. When omitted, a credential is created
    /// from <see cref="IAzureQueueOption.ManagedIdentityClientId"/>.
    /// </param>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="JsonSerializer"/> path and is not compatible with
    /// Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/> for AOT scenarios.
    /// </remarks>
    public PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger,
        TokenCredential? credential = null)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.QueueName);
        ValidateAuthenticationConfiguration();

        _credential = credential;
    }

    private void ValidateAuthenticationConfiguration()
    {
        if (!AzureServiceBusClientFactory.IsConfigured(_option))
        {
            throw new InvalidOperationException(
                "Azure Service Bus is not configured. Set FullyQualifiedNamespace for identity-based " +
                "authentication, or ConnectionString as a fallback.");
        }
    }

    /// <summary>
    /// Send message to queue.
    /// If sessionId is provided, messages retrieval is based on FIFO.
    /// If sessionId is Guid.Empty, sessionId is not added and retrieval is random.
    /// </summary>
    /// <param name="queueMessage">The queue message.</param>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>Task.</returns>
    /// <exception cref="ArgumentException">Entity should be type of IQueueMessage{TModel} instead of {queueMessage.GetType().FullName}</exception>
    public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId) // NOSONAR
    {
        if (queueMessage is { } model)
        {
            await using var client = AzureServiceBusClientFactory.Create(_option, _credential);
            var sender = client.CreateSender(_option.QueueName);

            var body = _jsonTypeInfo is not null
                ? JsonSerializer.Serialize(queueMessage, _jsonTypeInfo)
                : JsonSerializer.Serialize(queueMessage);

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(body));

            //If a session id is provided, add message to session.
            if (sessionId != Guid.Empty)
                message.SessionId = sessionId.ToString();

            //If a ContentType is provided, add ContentType to session.
            if (!string.IsNullOrEmpty(model.ContentType))
                message.ContentType = model.ContentType;

            //If a Tag is provided, add label to session.
            if (!string.IsNullOrEmpty(model.Tag))
                message.Subject = model.Tag;

            await sender.SendMessageAsync(message);
        }
        else
        {
            throw new ArgumentException("Entity should be type of IQueueMessage<TModel>");
        }
    }
}
