using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public abstract partial class BaseViewModel : PageModel, IBaseViewModel
    {
        public CultureInfo Culture { get; }
        public IMemoryCache Cache { get; }
        public IHostingEnvironment Environment { get; }
        
        public string Title { get; }
        public bool IsInitialized { get; private set; }

        #region Constructors
        public BaseViewModel(IHostingEnvironment environment, IMemoryCache cache, string currencySymbol, string title)
            : base()
        {
            Environment = environment;
            Cache = cache;

            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            Culture.NumberFormat.CurrencySymbol = $"{currencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            System.Threading.Thread.CurrentThread.CurrentCulture = Culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Culture;

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
        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
        }
        #endregion
    }
}
