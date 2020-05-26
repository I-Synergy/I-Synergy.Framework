using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store
{
    /// <summary>
    /// The interface for a property store factory
    /// </summary>
    public interface IPropertyStoreFactory
    {
        /// <summary>
        /// Creates/gets a property store for a file system
        /// </summary>
        /// <param name="fileSystem">The file system to get/create the property store for</param>
        /// <returns>The property store for the <paramref name="fileSystem"/></returns>
        IPropertyStore Create(IFileSystem fileSystem);
    }
}
