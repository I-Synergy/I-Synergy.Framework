using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.AspNetCore.WebDav.Routing
{
    /// <summary>
    /// The WebDAV HTTP PROPPATCH method
    /// </summary>
    public class HttpPropPatchAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "PROPPATCH" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPropPatchAttribute"/> class.
        /// </summary>
        public HttpPropPatchAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPropPatchAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpPropPatchAttribute([Required] string template)
            : base(_supportedMethods, template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));
        }
    }
}
