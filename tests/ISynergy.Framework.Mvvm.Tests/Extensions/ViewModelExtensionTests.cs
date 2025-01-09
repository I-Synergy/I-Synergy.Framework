using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Extensions.Tests;

[TestClass]
public class ViewModelExtensionTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;
    private Mock<ILanguageService> _mockLanguageService;

    [TestInitialize]
    public void Setup()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockLogger = new Mock<ILogger>();
        _mockLanguageService = new Mock<ILanguageService>();
    }

    // Test classes
    private class TestView : IView
    {
        public IViewModel ViewModel { get; set; }
        public bool IsEnabled { get; set; }

        public void Dispose()
        {
        }
    }
    private class TestViewModel : ViewModel
    {
        public TestViewModel(IScopedContextService scopedContextService, ICommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(scopedContextService, commonServices, logger, automaticValidation)
        {
        }
    }

    private class GenericViewModel<T> : TestViewModel
    {
        public GenericViewModel(IScopedContextService scopedContextService, ICommonServices commonServices, ILogger logger, bool automaticValidation = false)
            : base(scopedContextService, commonServices, logger, automaticValidation)
        {
        }
    }
    private interface ITestViewModel : IViewModel { }


    [DataTestMethod]
    [DataRow(typeof(ViewModels.MapsViewModel), "MapsViewModel")]
    [DataRow(typeof(ViewModels.NoteViewModel), "NoteViewModel")]
    public void GetNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
    {
        string result = viewModelType.GetViewModelName();
        Assert.AreEqual(expectedName, result);
    }

    [TestMethod]
    public void GetViewModelName_WithGenericType_ReturnsBaseNameWithoutGenericNotation()
    {
        // Arrange
        var type = typeof(GenericViewModel<string>);

        // Act
        var result = type.GetViewModelName();

        // Assert
        Assert.AreEqual("GenericViewModel", result);
    }

    [DataTestMethod]
    [DataRow(typeof(ViewModels.MapsViewModel), "ISynergy.Framework.Mvvm.ViewModels.MapsViewModel")]
    [DataRow(typeof(ViewModels.NoteViewModel), "ISynergy.Framework.Mvvm.ViewModels.NoteViewModel")]
    public void GetFullNameOfViewModelByTypeTest(Type viewModelType, string expectedName)
    {
        string result = viewModelType.GetViewModelFullName();
        Assert.AreEqual(expectedName, result);
    }

    [TestMethod]
    public void GetViewModelFullName_WithGenericType_ReturnsFullNameWithoutGenericNotation()
    {
        // Arrange
        var type = typeof(GenericViewModel<string>);

        // Act
        var result = type.GetViewModelFullName();

        // Assert
        Assert.IsTrue(result.EndsWith("GenericViewModel"));
        Assert.IsFalse(result.Contains("`"));
    }

    [TestMethod]
    public void GetViewModelName_FromInstance_ReturnsCorrectName()
    {
        // Arrange
        IViewModel viewModel = new TestViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        var result = viewModel.GetViewModelName();

        // Assert
        Assert.AreEqual("TestViewModel", result);
    }

    [TestMethod]
    public void GetViewModelFullName_FromInstance_ReturnsCorrectFullName()
    {
        // Arrange
        IViewModel viewModel = new TestViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        var result = viewModel.GetViewModelFullName();

        // Assert
        Assert.IsTrue(result.EndsWith("TestViewModel"));
    }

    [TestMethod]
    public void GetRelatedView_FromViewModelType_ReturnsViewName()
    {
        // Arrange
        var viewModelType = typeof(TestViewModel);

        // Act
        var result = viewModelType.GetRelatedView();

        // Assert
        Assert.AreEqual("TestView", result);
    }

    [TestMethod]
    public void GetRelatedView_FromViewModelInstance_ReturnsViewName()
    {
        // Arrange
        IViewModel viewModel = new TestViewModel(_mockScopedContextService.Object, _mockCommonServices.Object, _mockLogger.Object);

        // Act
        var result = viewModel.GetRelatedView();

        // Assert
        Assert.AreEqual("TestView", result);
    }

    [TestMethod]
    public void GetRelatedView_FromInterfaceType_RemovesIPrefix()
    {
        // Arrange
        var interfaceType = typeof(ITestViewModel);

        // Act
        var result = interfaceType.GetRelatedView();

        // Assert
        Assert.AreEqual("TestView", result);
    }

    [TestMethod]
    public void GetRelatedViewModel_FromViewType_ReturnsViewModelName()
    {
        // Arrange
        var viewType = typeof(TestView);

        // Act
        var result = viewType.GetRelatedViewModel();

        // Assert
        Assert.AreEqual("TestViewModel", result);
    }

    [TestMethod]
    public void GetRelatedViewModel_FromViewInstance_ReturnsViewModelName()
    {
        // Arrange
        IView view = new TestView();

        // Act
        var result = view.GetRelatedViewModel();

        // Assert
        Assert.AreEqual("TestViewModel", result);
    }

    [TestMethod]
    public void GetRelatedViewType_WithValidName_ReturnsType()
    {
        // Arrange
        var expectedType = typeof(TestView);
        var name = expectedType.FullName;

        // Act
        var result = name.GetRelatedViewType();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedType, result);
    }

    [TestMethod]
    public void GetRelatedViewType_WithInvalidName_ReturnsNull()
    {
        // Arrange
        var nonExistentName = "NonExistent.Type.Name";

        // Act
        var result = nonExistentName.GetRelatedViewType();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetRelatedViewType_WithShortName_FindsByName()
    {
        // Arrange
        var name = "TestView";

        // Act
        var result = name.GetRelatedViewType();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(typeof(TestView), result);
    }
}
