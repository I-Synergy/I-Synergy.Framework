using NugetUnlister.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NugetUnlister.Abstractions
{
    public interface INugetService
    {
        Task<NugetResponse> GetIndexAsync(string packageId, CancellationToken cancellationToken = default);
        Task<List<PackageVersion>> ListVersionAsync(string packageId, CancellationToken cancellationToken = default);
        Task UnlistPackageAsync(string packageId, string version, CancellationToken cancellationToken = default);
    }
}
