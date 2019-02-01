using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ISynergy.Services;
using ISynergy.Framework.Tests.Base;
using ISynergy.Functions;

namespace ISynergy.Framework.Tests.Functions
{
    [Collection("Localization")]
    public class LocalizationFunctionsTests : IntegrationTest
    {
        private readonly IContext Context;
        private readonly IBaseSettingsService Settings;

        public LocalizationFunctionsTests()
        {
            Context = new Context();
            Settings = new SettingsService();
        }

        [Fact]
        public void CheckIfCultureIsSetToDutchTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("nl");

            Assert.Equal("nl", Settings.Application_Culture);
        }

        [Fact]
        public void CheckIfCultureIsSetToEnglishTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("en");

            Assert.Equal("en", Settings.Application_Culture);
        }

        [Fact]
        public void CheckIfCurrencyIsSetToEuroTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("nl");

            Assert.Equal("€", Context.CurrencySymbol);
        }

        [Fact]
        public void CheckIfCurrencyIsSetToDollarTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("en");

            Assert.Equal("$", Context.CurrencySymbol);
        }
    }
}