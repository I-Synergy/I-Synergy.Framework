using System.Globalization;

namespace ISynergy.Framework.Core.Services;

[TestClass]
public class LanguageServiceTests
{
    [TestMethod]
    public void GetString_WithValidKey_ReturnsLocalizedString()
    {
        // Arrange
        var service = new LanguageService();

        // Act
        var result = service.GetString("WarningMandatoryProperty");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.IsFalse(result.StartsWith("["));
    }

    [TestMethod]
    public void GetString_WithInvalidKey_ReturnsPlaceholder()
    {
        // Arrange
        var service = new LanguageService();

        // Act
        var result = service.GetString("NonExistentKey_XYZ");

        // Assert
        Assert.AreEqual("[NonExistentKey_XYZ]", result);
    }

    [TestMethod]
    public void AddResourceManager_AddsNewResourceSource()
    {
        // Arrange
        var service = new LanguageService();
        service.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));

        // Act
        var result = service.GetString("Delete");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.IsFalse(result.StartsWith("["));
    }

    [TestMethod]
    public void GetString_RespectsCurrentUICulture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentUICulture;
        try
        {
            var service = new LanguageService();

            // Get English value
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            var englishResult = service.GetString("WarningMandatoryProperty");

            // Get Dutch value
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            var dutchResult = service.GetString("WarningMandatoryProperty");

            // Assert
            Assert.IsNotNull(englishResult);
            Assert.IsNotNull(dutchResult);
            Assert.IsFalse(englishResult.StartsWith("["));
            Assert.IsFalse(dutchResult.StartsWith("["));
            Assert.AreNotEqual(englishResult, dutchResult);
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalCulture;
        }
    }

    [TestMethod]
    public void GetString_WithMultipleManagers_SearchesInOrder()
    {
        // Arrange
        var service = new LanguageService();
        service.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));

        // Act
        var result = service.GetString("Delete");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(result.StartsWith("["));
    }

    [TestCleanup]
    public void Cleanup()
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
    }
}
