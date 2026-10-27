using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace ISynergy.Framework.UI.WPF.Services;

[TestClass]
public class ThemeServiceTests
{
    private const string AccentColor = "#0078D4";

    [TestMethod]
    public void Theme_Default_ResolvesToAnExplicitTheme()
    {
        var service = CreateService(Themes.Default);

        Assert.AreNotEqual(Themes.Default, service.Theme,
            "Themes.Default must be resolved to the theme the operating system is using");
        Assert.AreEqual(service.Theme == Themes.Light, service.IsLightThemeEnabled,
            "IsLightThemeEnabled must agree with the resolved Theme");
    }

    [TestMethod]
    public void Theme_ExplicitLight_IsNotOverriddenByTheOperatingSystem()
    {
        var service = CreateService(Themes.Light);

        Assert.AreEqual(Themes.Light, service.Theme);
        Assert.IsTrue(service.IsLightThemeEnabled);
    }

    [TestMethod]
    public void Theme_ExplicitDark_IsNotOverriddenByTheOperatingSystem()
    {
        var service = CreateService(Themes.Dark);

        Assert.AreEqual(Themes.Dark, service.Theme);
        Assert.IsFalse(service.IsLightThemeEnabled);
    }

    [TestMethod]
    public void SetTheme_ExplicitChoice_PersistsOverDefault()
    {
        var settings = new Mock<ISettingsService>();
        var localSettings = CreateLocalSettings(Themes.Default);
        settings.SetupGet(s => s.LocalSettings).Returns(localSettings.Object);

        var service = CreateService(settings);

        service.SetTheme(Themes.Dark);

        Assert.AreEqual(Themes.Dark, localSettings.Object.Theme);
        settings.Verify(s => s.SaveLocalSettings(), Times.AtLeastOnce);
    }

    private static ThemeService CreateService(Themes theme) =>
        CreateService(CreateSettings(theme));

    private static ThemeService CreateService(Mock<ISettingsService> settings) =>
        new(settings.Object, Microsoft.Extensions.Options.Options.Create(new ThemeColorOptions()), NullLogger<ThemeService>.Instance);

    private static Mock<ISettingsService> CreateSettings(Themes theme)
    {
        var localSettings = CreateLocalSettings(theme);
        var settings = new Mock<ISettingsService>();
        settings.SetupGet(s => s.LocalSettings).Returns(localSettings.Object);
        return settings;
    }

    private static Mock<ILocalSettings> CreateLocalSettings(Themes theme)
    {
        var localSettings = new Mock<ILocalSettings>();
        localSettings.SetupAllProperties();
        localSettings.Object.Theme = theme;
        localSettings.Object.Color = AccentColor;
        return localSettings;
    }
}
