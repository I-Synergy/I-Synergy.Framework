using System.ComponentModel;
using System.Globalization;

namespace ISynergy.Framework.Globalization.Services;

[TestClass]
public class LocalizedStringsTests
{
    [TestCleanup]
    public void Cleanup()
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
    }

    [TestMethod]
    public void Default_ReturnsSingletonInstance()
    {
        // Act
        var instance1 = LocalizedStrings.Default;
        var instance2 = LocalizedStrings.Default;

        // Assert
        Assert.IsNotNull(instance1);
        Assert.IsNotNull(instance2);
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void Indexer_WithValidKey_ReturnsLocalizedString()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var key = "WarningMandatoryProperty";

        // Act
        var result = service[key];

        // Assert
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.IsFalse(result.StartsWith("["));
    }

    [TestMethod]
    public void Indexer_WithInvalidKey_ReturnsPlaceholder()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var key = "NonExistentKey_XYZ";

        // Act
        var result = service[key];

        // Assert
        Assert.AreEqual("[NonExistentKey_XYZ]", result);
    }

    [TestMethod]
    public void Refresh_RaisesPropertyChangedForItem()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var propertyChanged = false;
        var propertyName = "";

        service.PropertyChanged += (sender, e) =>
        {
            propertyChanged = true;
            propertyName = e.PropertyName;
        };

        // Act
        service.Refresh();

        // Assert
        Assert.IsTrue(propertyChanged);
        Assert.AreEqual("Item[]", propertyName);
    }

    [TestMethod]
    public void Refresh_RaisesChangedEvent()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var changedRaised = false;

        service.Changed += (sender, e) =>
        {
            changedRaised = true;
        };

        // Act
        service.Refresh();

        // Assert
        Assert.IsTrue(changedRaised);
    }

    [TestMethod]
    public void Refresh_RaisesBothPropertyChangedAndChangedEvents()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var propertyChangedRaised = false;
        var changedRaised = false;

        service.PropertyChanged += (sender, e) =>
        {
            propertyChangedRaised = true;
        };

        service.Changed += (sender, e) =>
        {
            changedRaised = true;
        };

        // Act
        service.Refresh();

        // Assert
        Assert.IsTrue(propertyChangedRaised);
        Assert.IsTrue(changedRaised);
    }

    [TestMethod]
    public void Indexer_ReflectsCurrentCulture()
    {
        // Arrange
        var service = LocalizedStrings.Default;
        var originalCulture = CultureInfo.CurrentUICulture;

        try
        {
            // Act: Get string in English
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            var englishResult = service["WarningMandatoryProperty"];

            // Act: Get string in Dutch
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            var dutchResult = service["WarningMandatoryProperty"];

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
}
