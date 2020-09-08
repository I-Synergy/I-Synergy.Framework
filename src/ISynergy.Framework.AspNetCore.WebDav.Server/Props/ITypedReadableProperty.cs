using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props
{
    /// <summary>
    /// An interface for a typed readable property
    /// </summary>
    /// <typeparam name="T">The underlying type of the <see cref="System.Xml.Linq.XElement"/></typeparam>
    public interface ITypedReadableProperty<T> : IUntypedReadableProperty
    {
        /// <summary>
        /// Gets the underlying typed value
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The underlying typed value</returns>
        Task<T> GetValueAsync(CancellationToken ct);
    }
}
