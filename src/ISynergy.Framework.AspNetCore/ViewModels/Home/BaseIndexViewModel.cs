using ISynergy.Framework.AspNetCore.ViewModels.Base;
using ISynergy.Framework.Core.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace ISynergy.Framework.AspNetCore.ViewModels.Home
{
    /// <summary>
    /// Class BaseIndexViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public abstract class BaseIndexViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIndexViewModel"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="currencySymbol">The currency symbol.</param>
        /// <param name="title">The title.</param>
        protected BaseIndexViewModel(
            IWebHostEnvironment environment,
            IMemoryCache cache,
            string currencySymbol,
            string title)
            : base(environment, cache, currencySymbol, title)
        {
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [Description("Version")]
        public string Version { get; set; } = string.Empty;
    }
}
