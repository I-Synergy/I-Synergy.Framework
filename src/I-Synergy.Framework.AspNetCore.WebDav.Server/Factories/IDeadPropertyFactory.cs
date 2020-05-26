using System.Xml.Linq;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead
{
    /// <summary>
    /// The interface for a dead property factory
    /// </summary>
    public interface IDeadPropertyFactory
    {
        /// <summary>
        /// Creates a new dead property instance
        /// </summary>
        /// <param name="store">The property store to store this property</param>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="name">The name of the dead property to create</param>
        /// <returns>The created dead property instance</returns>
        IDeadProperty Create(IPropertyStore store, IEntry entry, XName name);

        /// <summary>
        /// Creates a new dead property instance
        /// </summary>
        /// <param name="store">The property store to store this property</param>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="element">The element to intialize the dead property with</param>
        /// <returns>The created dead property instance</returns>
        IDeadProperty Create(IPropertyStore store, IEntry entry, XElement element);
    }
}
