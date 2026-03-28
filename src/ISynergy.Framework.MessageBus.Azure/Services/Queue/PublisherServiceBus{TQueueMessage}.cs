using Azure.Messaging.ServiceBus;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.MessageBus.Abstractions;
using ISynergy.Framework.MessageBus.Abstractions.Messages.Base;
using ISynergy.Framework.MessageBus.Abstractions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
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
    where TOption : class, IQueueOption, new()
{
    private readonly TOption _option;
    private readonly ILogger _logger;
    private readonly JsonTypeInfo<TQueueMessage>? _jsonTypeInfo;

    /// <summary>
    /// Constructor of service bus. Uses AOT-safe source-generated JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="jsonTypeInfo">
    /// The <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TQueueMessage"/>, obtained from a
    /// <c>[JsonSerializable]</c>-attributed <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>. Required for Native AOT publishing.
    /// </param>
    public PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger,
        JsonTypeInfo<TQueueMessage> jsonTypeInfo)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
        _jsonTypeInfo = jsonTypeInfo;
    }

    /// <summary>
    /// Constructor of service bus. Uses reflection-based JSON serialization.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// This overload uses the reflection-based <see cref="JsonSerializer"/> path and is not compatible with
    /// Native AOT publishing. Use the overload that accepts a <see cref="JsonTypeInfo{T}"/> for AOT scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Serialization uses reflection. Pass a JsonTypeInfo<TQueueMessage> for AOT compatibility.")]
    [RequiresDynamicCode("Serialization uses dynamic code generation. Pass a JsonTypeInfo<TQueueMessage> for AOT compatibility.")]
    public PublisherServiceBus(
        IOptions<TOption> options,
        ILogger<PublisherServiceBus<TQueueMessage, TOption>> logger)
    {
        _option = options.Value;

        Argument.IsNotNullOrEmpty(_option.ConnectionString);
        Argument.IsNotNullOrEmpty(_option.QueueName);

        _logger = logger;
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
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Reflection path is only reachable when _jsonTypeInfo is null, which requires the [RequiresUnreferencedCode] constructor.")]
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AOT", "IL3050", Justification = "Reflection path is only reachable when _jsonTypeInfo is null, which requires the [RequiresDynamicCode] constructor.")]
    public virtual async Task SendMessageAsync(TQueueMessage queueMessage, Guid sessionId)
    {
        if (queueMessage is { } model)
        {
            await using var client = new ServiceBusClient(_option.ConnectionString);
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
