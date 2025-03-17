using ISynergy.Framework.AspNetCore.Globalization.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.Globalization.Options.Tests;

[TestClass]
public class GlobalizationOptionsTests
{
    [TestMethod]
    public void GlobalizationOptions_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new GlobalizationOptions();

        // Assert
        Assert.IsNull(options.DefaultCulture);
        Assert.IsNull(options.SupportedCultures);
        Assert.AreEqual(RequestCultureProviderTypes.Route, options.ProviderType);
    }

    [TestMethod]
    public void GlobalizationOptions_SetValues_AreCorrectlyStored()
    {
        // Arrange
        var options = new GlobalizationOptions
        {
            DefaultCulture = "en-US",
            SupportedCultures = new[] { "en-US", "nl-NL", "fr-FR" },
            ProviderType = RequestCultureProviderTypes.AcceptLanguageHeader
        };

        // Act & Assert
        Assert.AreEqual("en-US", options.DefaultCulture);
        Assert.AreEqual(3, options.SupportedCultures.Length);
        Assert.AreEqual("en-US", options.SupportedCultures[0]);
        Assert.AreEqual("nl-NL", options.SupportedCultures[1]);
        Assert.AreEqual("fr-FR", options.SupportedCultures[2]);
        Assert.AreEqual(RequestCultureProviderTypes.AcceptLanguageHeader, options.ProviderType);
    }
}
