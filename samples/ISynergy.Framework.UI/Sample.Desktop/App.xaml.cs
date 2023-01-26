using ISynergy.Framework.UI;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : BaseApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
            : base()
        {
            InitializeComponent();
        }

        public override Task InitializeApplicationAsync()
        {
            _logger.LogInformation("Starting initialization of application");

            var culture = CultureInfo.CurrentCulture;
            var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
            numberFormat.CurrencyNegativePattern = 1;
            
            _context.NumberFormat = numberFormat;

            _logger.LogInformation("Loading theme");
            _themeService.SetStyle();

            _logger.LogInformation("Setting up main page.");
            _logger.LogInformation("Finishing initialization of application");

            return Task.CompletedTask;
        }
    }
}
