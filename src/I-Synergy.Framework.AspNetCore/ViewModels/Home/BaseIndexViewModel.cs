using ISynergy.Framework.AspNetCore.ViewModels.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.AspNetCore.ViewModels.Home
{
    public abstract class BaseIndexViewModel : BaseViewModel
    {
        protected BaseIndexViewModel(
            IWebHostEnvironment environment,
            IMemoryCache cache,
            string currencySymbol,
            string title)
            : base(environment, cache, currencySymbol, title)
        {
        }

        [Display(Name = "Version")]
        public string Version { get; set; } = string.Empty;
    }
}
