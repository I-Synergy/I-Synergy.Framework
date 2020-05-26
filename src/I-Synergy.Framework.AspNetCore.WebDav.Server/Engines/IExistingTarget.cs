using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines
{
    /// <summary>
    /// The interface for an existing target
    /// </summary>
    public interface IExistingTarget : ITarget
    {
        /// <summary>
        /// Sets the properties of an existing target
        /// </summary>
        /// <param name="properties">The source properties</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The list of property names that couldn't be set</returns>
        Task<IReadOnlyCollection<XName>> SetPropertiesAsync(IEnumerable<IUntypedWriteableProperty> properties, CancellationToken cancellationToken);
    }
}
