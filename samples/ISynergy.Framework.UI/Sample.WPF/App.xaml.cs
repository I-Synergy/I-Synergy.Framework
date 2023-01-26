using ISynergy.Framework.UI;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
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
