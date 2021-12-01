using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Interceptors
{
    public interface ISyncInterceptor : IDisposable
    {
    }

    public interface ISyncInterceptor<T> : ISyncInterceptor
    {
        Task RunAsync(T args, CancellationToken cancellationToken);
    }

    public interface ISyncInterceptor2 : ISyncInterceptor
    {
        Task RunAsync(object args, CancellationToken cancellationToken);
    }

}
