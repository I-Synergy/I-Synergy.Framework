using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Live
{
    /// <summary>
    /// The interface for a live property
    /// </summary>
    public interface ILiveProperty : IUntypedReadableProperty
    {
        /// <summary>
        /// Determines whether the underlying value is valid
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns><see langword="true"/> when the underlying is valid</returns>
        Task<bool> IsValidAsync(CancellationToken cancellationToken);
    }
}
