using Amazon.S3;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.EventSourcing.Storage.Abstractions;
using ISynergy.Framework.EventSourcing.Storage.S3.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.EventSourcing.Storage.S3.Extensions;

/// <summary>
/// Extension methods for registering the AWS S3 event archive back-end.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="S3EventArchiveStorage"/> as the <see cref="IEventArchiveStorage"/>
    /// implementation using the provided <see cref="AmazonS3Client"/> and bucket name prefix.
    /// </summary>
    /// <remarks>
    /// Prerequisite: <see cref="AmazonS3Client"/> must already be registered in DI
    /// (e.g. <c>services.AddSingleton(new AmazonS3Client(credentials, config))</c>).
    /// <para>
    /// <strong>Tenant isolation:</strong> Each tenant receives its own bucket named
    /// <c>{bucketNamePrefix}-{tenantId}</c>. Buckets are created on demand.
    /// The object key within each bucket follows: <c>{streamType}/{streamId}/{vFrom}-{vTo}.json</c>.
    /// </para>
    /// <para>
    /// <strong>Bucket naming:</strong> <paramref name="bucketNamePrefix"/> must be lowercase,
    /// contain only letters, digits and hyphens, and be short enough that
    /// <c>{prefix}-{36-char-guid}</c> fits within the 63-character S3 bucket name limit
    /// (i.e. max 26 characters).
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="bucketNamePrefix">
    /// Prefix prepended to every per-tenant bucket name (e.g. <c>"archive"</c>).
    /// </param>
    /// <returns>The <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddS3EventArchiveStorage(
        this IServiceCollection services,
        string bucketNamePrefix)
    {
        Argument.IsNotNull(services);
        Argument.IsNotNullOrEmpty(bucketNamePrefix);

        services.AddScoped<IEventArchiveStorage>(sp =>
            new S3EventArchiveStorage(
                sp.GetRequiredService<AmazonS3Client>(),
                bucketNamePrefix,
                sp.GetRequiredService<ILogger<S3EventArchiveStorage>>()));

        return services;
    }
}
