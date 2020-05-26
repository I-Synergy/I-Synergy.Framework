using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.ViewModels.Base
{
    public abstract class BaseViewModel : PageModel, IBaseViewModel
    {
        public CultureInfo Culture { get; }
        public IMemoryCache Cache { get; }
        public IWebHostEnvironment Environment { get; }

        public string Title { get; }
        public bool IsInitialized { get; private set; }

        #region Constructors
        protected BaseViewModel(IWebHostEnvironment environment, IMemoryCache cache, string currencySymbol, string title)
        {
            Environment = environment;
            Cache = cache;

            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            Culture.NumberFormat.CurrencySymbol = $"{currencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = Culture;

            Title = title;

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
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        //~ObservableClass()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.
        }
        #endregion
    }
}
