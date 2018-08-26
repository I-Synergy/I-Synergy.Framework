using ISynergy.Services;
using ISynergy.ViewModels.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISynergy.ViewModels.Home
{
    public abstract class BaseIndexViewModel : BaseViewModel
    {
        public BaseIndexViewModel(IFactoryService factory, IHostingEnvironment environment, IMemoryCache cache)
            :base(factory, environment, cache)
        {
        }

        [Display(Name = "Version")]
        public string Version { get; set; }
    }
}
