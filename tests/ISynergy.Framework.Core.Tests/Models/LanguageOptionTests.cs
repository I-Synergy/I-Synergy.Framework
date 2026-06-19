using System.Globalization;

namespace ISynergy.Framework.Core.Models;

[TestClass]
public class LanguageOptionTests
{
    [TestMethod]
    public void Constructor_WithCultureInfo_SetsProperties()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("nl-NL");
        // Act
        var option = new LanguageOption(culture);
        // Assert
        Assert.AreEqual(culture, option.CultureInfo);
        Assert.AreEqual(culture.NativeName, option.NativeName);
        Assert.AreEqual(culture.EnglishName, option.EnglishName);
        Assert.AreEqual(culture.Name, option.Name);
        Assert.AreEqual(culture.TwoLetterISOLanguageName, option.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(() => new LanguageOption(null!));
    }

    [TestMethod]
    public void IsSelected_SetToTrue_FiresSelectedEvent()
    {
        // Arrange
        var option = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        bool eventFired = false;
        option.Selected += (_, _) => eventFired = true;
        // Act
        option.IsSelected = true;
        // Assert
        Assert.IsTrue(option.IsSelected);
        Assert.IsTrue(eventFired);
    }

    [TestMethod]
    public void IsSelected_SetToFalse_DoesNotFireSelectedEvent()
    {
        // Arrange
        var option = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        bool eventFired = false;
        option.Selected += (_, _) => eventFired = true;
        // Act
        option.IsSelected = false;
        // Assert
        Assert.IsFalse(option.IsSelected);
        Assert.IsFalse(eventFired);
    }

    [TestMethod]
    public void IsSelected_SetToTrueThenFalse_FiresSelectedOnlyOnce()
    {
        // Arrange
        var option = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        int fireCount = 0;
        option.Selected += (_, _) => fireCount++;
        // Act
        option.IsSelected = true;
        option.IsSelected = false;
        option.IsSelected = true;
        // Assert
        Assert.AreEqual(2, fireCount);
    }

    [TestMethod]
    public void IsSelected_SetSameValue_DoesNotFireEvent()
    {
        // Arrange
        var option = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        option.IsSelected = true;
        int fireCount = 0;
        option.Selected += (_, _) => fireCount++;
        // Act
        option.IsSelected = true; // same value
        // Assert
        Assert.AreEqual(0, fireCount);
    }

    [TestMethod]
    public void Equals_SameLanguage_ReturnsTrue()
    {
        // Arrange
        var option1 = new LanguageOption(CultureInfo.GetCultureInfo("nl"));
        var option2 = new LanguageOption(CultureInfo.GetCultureInfo("nl-NL")); // same two-letter
        // Act
        // Assert
        Assert.IsTrue(option1.Equals(option2));
    }

    [TestMethod]
    public void Equals_DifferentLanguage_ReturnsFalse()
    {
        // Arrange
        var option1 = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        var option2 = new LanguageOption(CultureInfo.GetCultureInfo("nl"));
        // Act
        // Assert
        Assert.IsFalse(option1.Equals(option2));
    }

    [TestMethod]
    public void GetHashCode_SameLanguage_ReturnsSameHashCode()
    {
        // Arrange
        var option1 = new LanguageOption(CultureInfo.GetCultureInfo("nl"));
        var option2 = new LanguageOption(CultureInfo.GetCultureInfo("nl-NL"));
        // Act
        // Assert
        Assert.AreEqual(option1.GetHashCode(), option2.GetHashCode());
    }

    [TestMethod]
    public void PropertyChanged_RaisesWhenIsSelectedChanges()
    {
        // Arrange
        var option = new LanguageOption(CultureInfo.GetCultureInfo("en"));
        string? changedProperty = null;
        option.PropertyChanged += (_, args) => changedProperty = args.PropertyName;
        // Act
        option.IsSelected = true;
        // Assert
        Assert.AreEqual(nameof(LanguageOption.IsSelected), changedProperty);
    }

    [TestMethod]
    public void ToString_ReturnsNativeName()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de");
        var option = new LanguageOption(culture);
        // Act
        // Assert
        Assert.AreEqual(culture.NativeName, option.ToString());
    }
}
