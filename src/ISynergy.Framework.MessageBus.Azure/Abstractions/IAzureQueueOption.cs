using ISynergy.Framework.MessageBus.Abstractions.Options;

namespace ISynergy.Framework.MessageBus.Azure.Abstractions;

/// <summary>
/// Azure Service Bus specific queue options.
/// </summary>
internal interface IAzureQueueOption : IQueueOption
{
    /// <summary>
    /// Gets the fully qualified Service Bus namespace, for example
    /// <c>mynamespace.servicebus.windows.net</c>. When set, identity-based authentication is used
    /// and takes precedence over <see cref="IQueueOption.ConnectionString"/>.
    /// </summary>
    string? FullyQualifiedNamespace { get; }

    /// <summary>
    /// Gets the client identifier of a user-assigned managed identity.
    /// When empty, the system-assigned identity (or another <c>DefaultAzureCredential</c> source)
    /// is used.
    /// </summary>
    string? ManagedIdentityClientId { get; }
}
