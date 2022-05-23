using System.Xml.Linq;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Model
{
    /// <summary>
    /// Utility properties for the WebDAV XML
    /// </summary>
    public static class WebDavXml
    {
        private const string WebDavNamespaceName = "DAV:";

        /// <summary>
        /// Gets the WebDAV namespace
        /// </summary>
        public static XNamespace Dav { get; } = XNamespace.Get(WebDavNamespaceName);
    }
}
