using ISynergy.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract partial class BaseViewModel : PageModel, IBaseViewModel
    {
        public CultureInfo Culture { get; protected set; }

        public IFactoryService _factory { get; protected set; }
        public IMemoryCache _cache { get; protected set; }
        public IHostingEnvironment _environment { get; protected set; }
        
        public virtual string Title { get; set; }
        public bool IsInitialized { get; set; }

        #region Constructors
        public BaseViewModel(IFactoryService factory, IHostingEnvironment environment, IMemoryCache cache)
            : base()
        {
            _factory = factory;
            _environment = environment;
            _cache = cache;

            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            Culture.NumberFormat.CurrencySymbol = $"{_factory.CurrencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            System.Threading.Thread.CurrentThread.CurrentCulture = Culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Culture;

            //Title = _factory.Tenant.Name;

            IsInitialized = false;
        }

        public virtual Task InitializeAsync()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
            }

            return Task.FromResult(IsInitialized);
        }
        #endregion

        #region IDisposable Support
        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
        }
        #endregion
    }
}
