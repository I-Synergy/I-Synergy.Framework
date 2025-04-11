using NugetUnlister.Models;

namespace NugetUnlister.Abstractions;

public interface INugetService
{
    Task<NugetResponse?> GetIndexAsync(string packageId, CancellationToken cancellationToken = default);
    Task<List<PackageVersion>> ListVersionAsync(string packageId, CancellationToken cancellationToken = default);
    Task UnlistPackageAsync(string packageId, string version, CancellationToken cancellationToken = default);
}
