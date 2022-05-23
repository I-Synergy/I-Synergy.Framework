using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.AspNetCore.WebDav.Routing
{
    /// <summary>
    /// The WebDAV HTTP COPY method
    /// </summary>
    public class HttpCopyAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "COPY" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCopyAttribute"/> class.
        /// </summary>
        public HttpCopyAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCopyAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpCopyAttribute([Required] string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
        }
    }
}
