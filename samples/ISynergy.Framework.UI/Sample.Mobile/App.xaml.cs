using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.UI;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Sample
{
    public partial class App : BaseApplication
    {
        public App()
            : base()
        {
            InitializeComponent();
            InitializeApplication();
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

            MainPage = ServiceLocator.Default.GetInstance<AppShell>();

            _logger.LogInformation("Finishing initialization of application");

            return Task.CompletedTask;
        }
    }
}