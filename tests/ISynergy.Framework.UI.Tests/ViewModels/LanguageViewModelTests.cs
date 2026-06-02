using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;
using System.Globalization;

namespace ISynergy.Framework.UI.ViewModels;

[TestClass]
public class LanguageViewModelTests
{
    private Mock<ICommonServices> _mockCommonServices = null!;
    private Mock<ISettingsService> _mockSettingsService = null!;
    private Mock<ISupportedCulturesService> _mockSupportedCulturesService = null!;
    private Mock<ILanguageService> _mockLanguageService = null!;
    private Mock<ILogger<LanguageViewModel>> _mockLogger = null!;
    private Mock<IMessengerService> _mockMessengerService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _mockLanguageService = new Mock<ILanguageService>();
        _mockLanguageService
            .Setup(x => x.GetString(It.IsAny<string>()))
            .Returns<string>(key => $"[{key}]");

        _mockMessengerService = new Mock<IMessengerService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.LanguageService).Returns(_mockLanguageService.Object);
        _mockCommonServices.SetupGet(s => s.MessengerService).Returns(_mockMessengerService.Object);

        _mockSettingsService = new Mock<ISettingsService>();
        _mockSupportedCulturesService = new Mock<ISupportedCulturesService>();
        _mockLogger = new Mock<ILogger<LanguageViewModel>>();
    }

    [TestMethod]
    public void Constructor_WithSupportedCultures_BuildsSupportedCulturesList()
    {
        // Arrange
        var cultures = new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("nl"),
            CultureInfo.GetCultureInfo("de")
        };
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures).Returns(cultures.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported(It.IsAny<string>())).Returns(true);
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));

        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub("en"));

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.AreEqual(3, vm.SupportedCultures.Count);
        Assert.AreEqual("en", vm.SupportedCultures[0].TwoLetterISOLanguageName);
        Assert.AreEqual("nl", vm.SupportedCultures[1].TwoLetterISOLanguageName);
        Assert.AreEqual("de", vm.SupportedCultures[2].TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithSavedLanguage_PreSelectsIt()
    {
        // Arrange
        var cultures = new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("nl")
        };
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures).Returns(cultures.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported("nl")).Returns(true);
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));

        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub("nl"));

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.IsNotNull(vm.SelectedItem);
        Assert.AreEqual("nl", vm.SelectedItem.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithUnsupportedSavedLanguage_FallsBackToDefault()
    {
        // Arrange
        var cultures = new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("nl")
        };
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures).Returns(cultures.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported("fr")).Returns(false); // fr not supported
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported("en")).Returns(true);
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));

        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub("fr")); // saved as fr but not supported

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.IsNotNull(vm.SelectedItem);
        Assert.AreEqual("en", vm.SelectedItem.TwoLetterISOLanguageName); // falls back to default
    }

    [TestMethod]
    public void Constructor_WithNullSavedLanguage_FallsBackToDefault()
    {
        // Arrange
        var cultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en"), CultureInfo.GetCultureInfo("nl") };
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures).Returns(cultures.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported(It.IsAny<string>())).Returns(false);
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));

        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub(null));

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.IsNotNull(vm.SelectedItem);
        Assert.AreEqual("en", vm.SelectedItem.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Constructor_WithEmptySavedLanguage_FallsBackToDefault()
    {
        // Arrange
        var cultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en") };
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures).Returns(cultures.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));

        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub(""));

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.IsNotNull(vm.SelectedItem);
        Assert.AreEqual("en", vm.SelectedItem.TwoLetterISOLanguageName);
    }

    [TestMethod]
    public void Title_ReturnsLocalizedString()
    {
        // Arrange
        _mockSupportedCulturesService.Setup(s => s.SupportedCultures)
            .Returns(new List<CultureInfo> { CultureInfo.GetCultureInfo("en") }.AsReadOnly());
        _mockSupportedCulturesService.Setup(s => s.IsCultureSupported(It.IsAny<string>())).Returns(true);
        _mockSupportedCulturesService.Setup(s => s.DefaultCulture).Returns(CultureInfo.GetCultureInfo("en"));
        _mockSettingsService.Setup(s => s.LocalSettings).Returns(new LocalSettingsStub("en"));

        // Act
        var vm = new LanguageViewModel(
            _mockCommonServices.Object,
            _mockSettingsService.Object,
            _mockSupportedCulturesService.Object,
            _mockLogger.Object);

        // Assert
        Assert.AreEqual("[Language]", vm.Title);
    }

    /// <summary>
    /// Minimal stub implementing ILocalSettings for test purposes.
    /// </summary>
    private sealed class LocalSettingsStub : ILocalSettings
    {
#pragma warning disable CS0067 // Required by ILocalSettings interface but not raised in this stub
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public string Language { get; set; }
        public bool IsFullscreen { get; set; }
        public string Color { get; set; } = string.Empty;
        public Themes Theme { get; set; }
        public bool IsLightThemeEnabled => Theme == Themes.Light;
        public bool IsAdvanced { get; set; }
        public bool ShowConsole { get; set; }
        public int MigrationVersion { get; set; }

        public LocalSettingsStub(string? language)
        {
            Language = language ?? string.Empty;
        }
    }
}
