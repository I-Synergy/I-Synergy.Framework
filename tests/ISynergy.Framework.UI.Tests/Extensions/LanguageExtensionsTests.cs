using ISynergy.Framework.UI.Extensions;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;

[TestClass]
public class LanguageExtensionsTests
{
    [TestCleanup]
    public void Cleanup()
    {
        // Reset to English after each test to avoid cross-test contamination
        var english = CultureInfo.GetCultureInfo("en");
        CultureInfo.DefaultThreadCurrentCulture = english;
        CultureInfo.DefaultThreadCurrentUICulture = english;
    }

    [TestMethod]
    public void SetLocalizationLanguage_WithCultureInfo_SetsBothThreadCultures()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("nl-NL");

        // Act
        culture.SetLocalizationLanguage();

        // Assert
        Assert.AreEqual("nl-NL", CultureInfo.DefaultThreadCurrentCulture?.Name);
        Assert.AreEqual("nl-NL", CultureInfo.DefaultThreadCurrentUICulture?.Name);
    }

    [TestMethod]
    public void SetLocalizationLanguage_WithValidString_SetsCulture()
    {
        // Arrange & Act
        "de".SetLocalizationLanguage();

        // Assert
        Assert.AreEqual("de", CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
        Assert.AreEqual("de", CultureInfo.DefaultThreadCurrentUICulture?.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void SetLocalizationLanguage_WithSpecificCultureString_SetsCulture()
    {
        // Arrange & Act
        "nl-BE".SetLocalizationLanguage();

        // Assert
        Assert.AreEqual("nl-BE", CultureInfo.DefaultThreadCurrentCulture?.Name);
        Assert.AreEqual("nl-BE", CultureInfo.DefaultThreadCurrentUICulture?.Name);
    }

    [TestMethod]
    public void SetLocalizationLanguage_WithEnglish_SetsEnglish()
    {
        // First set to non-English
        CultureInfo.GetCultureInfo("nl").SetLocalizationLanguage();

        // Then set back to English
        "en".SetLocalizationLanguage();

        Assert.AreEqual("en", CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
        Assert.AreEqual("en", CultureInfo.DefaultThreadCurrentUICulture?.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void SetLocalizationLanguage_WithNullOrEmptyString_DoesNotThrow()
    {
        // Should NOT throw — these are safe no-ops
        "".SetLocalizationLanguage();

        // Test with null — pass via nullable variable to avoid CS8600/CS8625
#pragma warning disable CS8600, CS8625
        ((string)null).SetLocalizationLanguage();
#pragma warning restore CS8600, CS8625

        // At minimum, no exception was thrown — verify culture is set (cleanup runs after)
        Assert.IsNotNull(CultureInfo.DefaultThreadCurrentCulture);
    }
}
