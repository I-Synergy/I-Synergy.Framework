using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ISynergy.Framework.MessageBus.Azure.Abstractions;

namespace ISynergy.Framework.MessageBus.Azure.Services.Queue;

/// <summary>
/// Creates <see cref="ServiceBusClient"/> instances using identity-based authentication when a
/// namespace is configured, falling back to the connection string.
/// </summary>
internal static class AzureServiceBusClientFactory
{
    /// <summary>
    /// Creates a Service Bus client for the supplied options.
    /// </summary>
    /// <param name="option">The queue options.</param>
    /// <param name="credential">Optional credential to use for identity-based authentication.</param>
    /// <returns>The configured <see cref="ServiceBusClient"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when neither a fully qualified namespace nor a connection string is configured.
    /// </exception>
    internal static ServiceBusClient Create(IAzureQueueOption option, TokenCredential? credential = null)
    {
        if (!string.IsNullOrWhiteSpace(option.FullyQualifiedNamespace))
        {
            return new ServiceBusClient(
                option.FullyQualifiedNamespace,
                credential ?? CreateCredential(option.ManagedIdentityClientId));
        }

        if (!string.IsNullOrWhiteSpace(option.ConnectionString))
            return new ServiceBusClient(option.ConnectionString);

        throw new InvalidOperationException(
            "Azure Service Bus is not configured. Set FullyQualifiedNamespace for identity-based " +
            "authentication, or ConnectionString as a fallback.");
    }

    /// <summary>
    /// Returns whether the options contain either identity or connection-string configuration.
    /// </summary>
    internal static bool IsConfigured(IAzureQueueOption option)
        => !string.IsNullOrWhiteSpace(option.FullyQualifiedNamespace)
           || !string.IsNullOrWhiteSpace(option.ConnectionString);

    /// <summary>
    /// Creates a credential for identity-based authentication.
    /// </summary>
    /// <remarks>
    /// <see cref="DefaultAzureCredential"/> probes several credential sources and is not fully
    /// trim/AOT safe. Applications publishing with <c>PublishAot</c> should pass an explicit
    /// <see cref="TokenCredential"/> or set <c>ManagedIdentityClientId</c> to use
    /// <see cref="ManagedIdentityCredential"/>.
    /// </remarks>
    private static TokenCredential CreateCredential(string? managedIdentityClientId)
        => string.IsNullOrWhiteSpace(managedIdentityClientId)
            ? new DefaultAzureCredential()
            : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(managedIdentityClientId));
}
