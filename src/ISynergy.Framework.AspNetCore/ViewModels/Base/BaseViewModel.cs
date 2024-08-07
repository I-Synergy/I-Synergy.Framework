using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace ISynergy.Framework.AspNetCore.ViewModels.Base;

/// <summary>
/// Class BaseViewModel.
/// Implements the <see cref="PageModel" />
/// Implements the <see cref="IBaseViewModel" />
/// </summary>
/// <seealso cref="PageModel" />
/// <seealso cref="IBaseViewModel" />
public abstract class BaseViewModel : PageModel, IBaseViewModel
{
    /// <summary>
    /// Gets the culture.
    /// </summary>
    /// <value>The culture.</value>
    public CultureInfo Culture { get; }
    /// <summary>
    /// Gets the cache.
    /// </summary>
    /// <value>The cache.</value>
    public IMemoryCache Cache { get; }
    /// <summary>
    /// Gets the environment.
    /// </summary>
    /// <value>The environment.</value>
    public IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
    public bool IsInitialized { get; private set; }

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <param name="cache">The cache.</param>
    /// <param name="currencySymbol">The currency symbol.</param>
    /// <param name="title">The title.</param>
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

    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public abstract Task InitializeAsync();
    #endregion

    #region IDisposable Support
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
