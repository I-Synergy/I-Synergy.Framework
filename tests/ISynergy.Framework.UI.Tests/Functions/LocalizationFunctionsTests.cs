using ISynergy.Framework.Windows.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Moq;
using ISynergy.Framework.Windows.Services;

namespace ISynergy.Framework.Windows.Tests
{
    [TestClass]
    public class LocalizationFunctionsTests
    {
        private readonly IContext Context;
        private readonly IApplicationSettingsService Settings;

        public LocalizationFunctionsTests()
        {
            Context = new Mock<IContext>().SetupAllProperties().Object;
            Settings = new ApplicationSettingsService();
        }

        [TestMethod]
        public void CheckIfCultureIsSetToDutchTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("nl");

            Assert.AreEqual("nl", Settings.Culture);
        }

        [TestMethod]
        public void CheckIfCultureIsSetToEnglishTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("en");

            Assert.AreEqual("en", Settings.Culture);
        }

        [TestMethod]
        public void CheckIfCurrencyIsSetToEuroTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("nl");

            Assert.AreEqual("€", Context.CurrencySymbol);
        }

        [TestMethod]
        public void CheckIfCurrencyIsSetToDollarTest()
        {
            var x = new LocalizationFunctions(Context, Settings);
            x.SetLocalizationLanguage("en");

            Assert.AreEqual("$", Context.CurrencySymbol);
        }
    }
}
