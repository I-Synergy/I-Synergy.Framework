using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers
{
    /// <summary>
    /// The handler for a HTTP method for a given WebDAV class
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Gets the HTTP methods that are processed by this handler
        /// </summary>
        IEnumerable<string> HttpMethods { get; }
    }
}
