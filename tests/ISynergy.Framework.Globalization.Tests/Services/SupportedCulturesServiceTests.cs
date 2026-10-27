using System.Globalization;

namespace ISynergy.Framework.Globalization.Services;

[TestClass]
public class SupportedCulturesServiceTests
{
    public SupportedCulturesServiceTests()
    {
    }

    [TestMethod]
    public void Constructor_WithNoCultures_InsertsEnglishAsDefault()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([]);
        // Assert
        Assert.AreEqual(1, service.SupportedCultures.Count);
        Assert.AreEqual("en", service.SupportedCultures[0].TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithEnglishAlreadyPresent_DoesNotDuplicate()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([CultureInfo.GetCultureInfo("en")]);
        // Assert
        Assert.AreEqual(1, service.SupportedCultures.Count);
        Assert.AreEqual("en", service.SupportedCultures[0].TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithEnglishNotFirst_MovesEnglishToFirst()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([
            CultureInfo.GetCultureInfo("nl"),
            CultureInfo.GetCultureInfo("de"),
            CultureInfo.GetCultureInfo("en")
        ]);
        // Assert
        Assert.AreEqual("en", service.SupportedCultures[0].TwoLetterISOLanguageName);
        Assert.AreEqual(3, service.SupportedCultures.Count);
    }

    [TestMethod]
    public void Constructor_WithMultipleCultures_PreservesOrder()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([
            CultureInfo.GetCultureInfo("nl"),
            CultureInfo.GetCultureInfo("de"),
            CultureInfo.GetCultureInfo("fr")
        ]);
        // Assert
        // English inserted at index 0, others preserve relative order
        Assert.AreEqual("en", service.SupportedCultures[0].TwoLetterISOLanguageName);
        Assert.AreEqual("nl", service.SupportedCultures[1].TwoLetterISOLanguageName);
        Assert.AreEqual("de", service.SupportedCultures[2].TwoLetterISOLanguageName);
        Assert.AreEqual("fr", service.SupportedCultures[3].TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithSpecificCulture_IncludesIt()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([
            CultureInfo.GetCultureInfo("nl-NL"),
            CultureInfo.GetCultureInfo("nl-BE")
        ]);
        // Assert
        Assert.AreEqual("en", service.SupportedCultures[0].TwoLetterISOLanguageName);
        Assert.AreEqual("nl-NL", service.SupportedCultures[1].Name);
        Assert.AreEqual("nl-BE", service.SupportedCultures[2].Name);
    }

    [TestMethod]
    public void DefaultCulture_AlwaysReturnsEnglish()
    {
        // Arrange
        // Act
        var service = new SupportedCulturesService([
            CultureInfo.GetCultureInfo("nl"),
            CultureInfo.GetCultureInfo("de")
        ]);
        // Assert
        Assert.AreEqual("en", service.DefaultCulture.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void IsCultureSupported_WithValidNeutralCulture_ReturnsTrue()
    {
        // Arrange
        var service = new SupportedCulturesService([CultureInfo.GetCultureInfo("nl")]);
        // Act
        // Assert
        Assert.IsTrue(service.IsCultureSupported("nl"));
        Assert.IsTrue(service.IsCultureSupported("NL")); // case insensitive
    }

    [TestMethod]
    public void IsCultureSupported_WithValidSpecificCulture_ReturnsTrue()
    {
        // Arrange
        var service = new SupportedCulturesService([CultureInfo.GetCultureInfo("nl-NL")]);
        // Act
        // Assert
        Assert.IsTrue(service.IsCultureSupported("nl-NL"));
        Assert.IsTrue(service.IsCultureSupported("nl")); // TwoLetterISOLanguageName match
    }

    [TestMethod]
    public void IsCultureSupported_WithUnsupportedCulture_ReturnsFalse()
    {
        // Arrange
        var service = new SupportedCulturesService([CultureInfo.GetCultureInfo("nl")]);
        // Act
        // Assert
        Assert.IsFalse(service.IsCultureSupported("de"));
        Assert.IsFalse(service.IsCultureSupported("fr"));
    }

    [TestMethod]
    public void IsCultureSupported_WithNullOrEmpty_ReturnsFalse()
    {
        // Arrange
        var service = new SupportedCulturesService([]);
        // Act
        // Assert
        Assert.IsFalse(service.IsCultureSupported(null!));
        Assert.IsFalse(service.IsCultureSupported(""));
        Assert.IsFalse(service.IsCultureSupported("  "));
    }

    [TestMethod]
    public void IsCultureSupported_EnglishAlwaysSupported()
    {
        // Arrange
        var service = new SupportedCulturesService([CultureInfo.GetCultureInfo("nl")]);
        // Act
        // Assert
        Assert.IsTrue(service.IsCultureSupported("en"));
    }
}
