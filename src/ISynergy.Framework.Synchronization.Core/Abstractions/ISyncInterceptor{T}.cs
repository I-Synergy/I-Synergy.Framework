using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Abstractions
{
    public interface ISyncInterceptor<T> : ISyncInterceptor
    {
        Task RunAsync(T args, CancellationToken cancellationToken);
    }
}
