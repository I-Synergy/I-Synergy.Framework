using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store
{
    /// <summary>
    /// Interface for a property store that stores the properties on the local file system
    /// </summary>
    public interface IFileSystemPropertyStore : IPropertyStore
    {
        /// <summary>
        /// Determines whether the given <paramref name="entry"/> should be ignored when the client performs a PROPFIND
        /// </summary>
        /// <param name="entry">The entry that needs to be checked if it should be ignored</param>
        /// <returns><see langword="true"/> when the entry should be ignored</returns>
        bool IgnoreEntry(IEntry entry);
    }
}
