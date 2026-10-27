using ISynergy.Framework.Globalization.Services;
using System.Globalization;

namespace ISynergy.Framework.Globalization.Extensions;

[TestClass]
public class CultureInfoExtensionsTests
{
    private CultureInfo? _originalCulture;
    private CultureInfo? _originalUICulture;

    [TestInitialize]
    public void Initialize()
    {
        _originalCulture = CultureInfo.CurrentCulture;
        _originalUICulture = CultureInfo.CurrentUICulture;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalCulture != null)
            CultureInfo.CurrentCulture = _originalCulture;
        if (_originalUICulture != null)
            CultureInfo.CurrentUICulture = _originalUICulture;
    }

    [TestMethod]
    public void ApplyCulture_WithValidCulture_SetsDefaultThreadCurrentCulture()
    {
        // Arrange
        var dutchCulture = CultureInfo.GetCultureInfo("nl");

        // Act
        var result = dutchCulture.ApplyCulture();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("nl", CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void ApplyCulture_WithValidCulture_SetsDefaultThreadCurrentUICulture()
    {
        // Arrange
        var dutchCulture = CultureInfo.GetCultureInfo("nl");

        // Act
        dutchCulture.ApplyCulture();

        // Assert
        Assert.AreEqual("nl", CultureInfo.DefaultThreadCurrentUICulture?.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void ApplyCulture_WithNullCulture_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exceptionThrown = false;
        try
        {
            ((CultureInfo)null!).ApplyCulture();
        }
        catch (ArgumentNullException)
        {
            exceptionThrown = true;
        }

        Assert.IsTrue(exceptionThrown, "ArgumentNullException was not thrown");
    }

    [TestMethod]
    public void ApplyCulture_WithConfigureCallback_AppliesCustomization()
    {
        // Arrange
        var dutchCulture = CultureInfo.GetCultureInfo("nl");
        var customSymbol = "€€";

        // Act
        dutchCulture.ApplyCulture(ci =>
        {
            ci.NumberFormat.CurrencySymbol = customSymbol;
        });

        // Assert
        Assert.AreEqual(customSymbol, CultureInfo.DefaultThreadCurrentCulture?.NumberFormat.CurrencySymbol);
    }

    [TestMethod]
    public void ApplyCulture_DoesNotMutateOriginalCulture()
    {
        // Arrange
        var originalCulture = CultureInfo.GetCultureInfo("nl");
        var originalSymbol = originalCulture.NumberFormat.CurrencySymbol;

        // Act
        originalCulture.ApplyCulture(ci =>
        {
            ci.NumberFormat.CurrencySymbol = "Custom";
        });

        // Assert
        Assert.AreEqual(originalSymbol, originalCulture.NumberFormat.CurrencySymbol);
    }

    [TestMethod]
    public void ApplyCulture_RefreshesLocalizedStrings()
    {
        // Arrange
        var dutchCulture = CultureInfo.GetCultureInfo("nl");
        var localizedStrings = LocalizedStrings.Default;
        var propertyChangedRaised = false;
        var changedRaised = false;

        localizedStrings.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "Item[]")
                propertyChangedRaised = true;
        };

        localizedStrings.Changed += (sender, e) =>
        {
            changedRaised = true;
        };

        // Act
        dutchCulture.ApplyCulture();

        // Assert
        Assert.IsTrue(propertyChangedRaised, "PropertyChanged event with 'Item[]' was not raised");
        Assert.IsTrue(changedRaised, "Changed event was not raised");
    }
}
