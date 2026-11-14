using ISynergy.Framework.MessageBus.Abstractions.Options;
using ISynergy.Framework.MessageBus.Azure.Abstractions;
using ISynergy.Framework.MessageBus.Options.Base;

namespace ISynergy.Framework.MessageBus.Azure.Options.Queue;

/// <summary>
/// Base options for Azure Service Bus queues.
/// </summary>
internal abstract class AzureQueueOptionBase : BaseQueueOption, IAzureQueueOption
{
    /// <summary>
    /// Gets or sets the fully qualified Service Bus namespace, for example
    /// <c>mynamespace.servicebus.windows.net</c>. When set, identity-based authentication is used
    /// and takes precedence over the connection string.
    /// </summary>
    public string? FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Gets or sets the client identifier of a user-assigned managed identity.
    /// When empty, the system-assigned identity (or another <c>DefaultAzureCredential</c> source)
    /// is used.
    /// </summary>
    public string? ManagedIdentityClientId { get; set; }
}
