using System;

namespace ISynergy.Framework.Core.Abstractions.Services
{
    /// <summary>
    /// Interface IVersionService
    /// </summary>
    public interface IVersionService
    {
        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        Version ProductVersion { get; }
    }
}
