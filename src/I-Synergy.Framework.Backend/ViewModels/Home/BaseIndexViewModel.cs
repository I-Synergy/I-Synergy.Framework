using ISynergy.ViewModels.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.ViewModels.Home
{
    public abstract class BaseIndexViewModel : BaseViewModel
    {
        public BaseIndexViewModel(
            IHostingEnvironment environment, 
            IMemoryCache cache, 
            string currencySymbol, 
            string title)
            :base(environment, cache, currencySymbol, title)
        {
        }

        [Display(Name = "Version")]
        public string Version { get; set; }
    }
}
