using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using System;

namespace ISynergy.Framework.AspNetCore.Attributes
{
    /// <summary>
    /// Class FormValueRequiredAttribute. This class cannot be inherited.
    /// Implements the <see cref="ActionMethodSelectorAttribute" />
    /// </summary>
    /// <seealso cref="ActionMethodSelectorAttribute" />
    public sealed class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// The name
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormValueRequiredAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FormValueRequiredAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Determines whether the action selection is valid for the specified route context.
        /// </summary>
        /// <param name="routeContext">The route context.</param>
        /// <param name="action">Information about the action.</param>
        /// <returns><see langword="true" /> if the action  selection is valid for the specified context;
        /// otherwise, <see langword="false" />.</returns>
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (string.Equals(routeContext.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(routeContext.HttpContext.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(routeContext.HttpContext.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(routeContext.HttpContext.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.IsNullOrEmpty(routeContext.HttpContext.Request.ContentType))
            {
                return false;
            }

            if (!routeContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return !string.IsNullOrEmpty(routeContext.HttpContext.Request.Form[_name]);
        }
    }
}
