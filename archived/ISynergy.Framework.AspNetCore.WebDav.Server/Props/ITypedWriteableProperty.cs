using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props
{
    /// <summary>
    /// A typed writeable property
    /// </summary>
    /// <typeparam name="T">The underlying type of the <see cref="System.Xml.Linq.XElement"/></typeparam>
    public interface ITypedWriteableProperty<T> : ITypedReadableProperty<T>, IUntypedWriteableProperty
    {
        /// <summary>
        /// Sets the underlying typed value
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>the async task</returns>
        Task SetValueAsync(T value, CancellationToken ct);
    }
}
