using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.AspNetCore.WebDav.Routing
{
    /// <summary>
    /// The WebDAV HTTP UNLOCK method
    /// </summary>
    public class HttpUnlockAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "UNLOCK" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpUnlockAttribute"/> class.
        /// </summary>
        public HttpUnlockAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpUnlockAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpUnlockAttribute([Required] string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
        }
    }
}
