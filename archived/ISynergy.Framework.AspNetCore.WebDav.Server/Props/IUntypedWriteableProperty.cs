using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props
{
    /// <summary>
    /// The interface for an untyped writeable property
    /// </summary>
    public interface IUntypedWriteableProperty : IUntypedReadableProperty
    {
        /// <summary>
        /// Sets the <see cref="XElement"/> for the property
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to be set</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The async task</returns>
        Task SetXmlValueAsync(XElement element, CancellationToken ct);
    }
}
