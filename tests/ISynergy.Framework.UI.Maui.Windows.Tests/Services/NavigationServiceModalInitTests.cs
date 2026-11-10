using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using MvvmIView = ISynergy.Framework.Mvvm.Abstractions.IView;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Tests for the extracted <c>InitializeModalViewModelAsync</c> helper on
/// <see cref="NavigationService"/>. The helper is deliberately independent of the
/// live MAUI modal stack so it can be exercised with a mocked <see cref="MvvmIView"/>
/// and <see cref="IViewModel"/>; only the guard (null view-model) and the
/// exception-routing paths are relevant here.
/// </summary>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class NavigationServiceModalInitTests
{
    private Mock<IExceptionHandlerService> _mockExceptionHandler = null!;
    private NavigationService _navigationService = null!;
    private MethodInfo _initializeModal = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        var mockCommonServices = new Mock<ICommonServices>();
        var mockLocalSettings = new Mock<ILocalSettings>();
        var mockLogger = new Mock<ILogger<NavigationService>>();

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

        _navigationService = new NavigationService(
            _mockExceptionHandler.Object,
            mockCommonServices.Object,
            mockLocalSettings.Object,
            mockScopeFactory.Object,
            mockLogger.Object);

        _initializeModal = typeof(NavigationService)
            .GetMethod("InitializeModalViewModelAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    private Task InvokeInitialize(MvvmIView view) =>
        (Task)_initializeModal.Invoke(_navigationService, new object?[] { view })!;

    [TestMethod]
    public void InitializeModalViewModelAsync_HasExpectedSignature()
    {
        Assert.IsNotNull(_initializeModal, "InitializeModalViewModelAsync should exist");
        Assert.IsTrue(_initializeModal.IsPrivate);
        Assert.IsFalse(_initializeModal.IsStatic);
        Assert.AreEqual(typeof(Task), _initializeModal.ReturnType);
    }

    [TestMethod]
    public async Task InitializeModalViewModelAsync_WithNullViewModel_ReturnsWithoutCallingHandler()
    {
        var view = new Mock<MvvmIView>();
        view.SetupGet(x => x.ViewModel).Returns((IViewModel)null!);

        await InvokeInitialize(view.Object);

        _mockExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Never);
    }

    [TestMethod]
    public async Task InitializeModalViewModelAsync_WhenInitializeSucceeds_DoesNotCallHandler()
    {
        var mockViewModel = new Mock<IViewModel>();
        mockViewModel.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);

        var view = new Mock<MvvmIView>();
        view.SetupGet(x => x.ViewModel).Returns(mockViewModel.Object);

        await InvokeInitialize(view.Object);

        mockViewModel.Verify(x => x.InitializeAsync(), Times.Once);
        _mockExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Never);
    }

    [TestMethod]
    public async Task InitializeModalViewModelAsync_WhenInitializeThrows_RoutesToExceptionHandler()
    {
        var expectedException = new InvalidOperationException("init failed");
        var mockViewModel = new Mock<IViewModel>();
        mockViewModel.Setup(x => x.InitializeAsync()).ThrowsAsync(expectedException);

        var view = new Mock<MvvmIView>();
        view.SetupGet(x => x.ViewModel).Returns(mockViewModel.Object);

        await InvokeInitialize(view.Object);

        _mockExceptionHandler.Verify(
            x => x.HandleException(It.Is<Exception>(e => e == expectedException)),
            Times.Once);
    }

    [TestMethod]
    public async Task InitializeModalViewModelAsync_WhenHandlerAlsoThrows_DoesNotPropagate()
    {
        var mockViewModel = new Mock<IViewModel>();
        mockViewModel.Setup(x => x.InitializeAsync()).ThrowsAsync(new Exception("init failed"));

        _mockExceptionHandler
            .Setup(x => x.HandleException(It.IsAny<Exception>()))
            .Throws(new InvalidOperationException("handler crashed"));

        var view = new Mock<MvvmIView>();
        view.SetupGet(x => x.ViewModel).Returns(mockViewModel.Object);

        // Should not throw even though both the init and the handler fail.
        await InvokeInitialize(view.Object);

        _mockExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>()), Times.Once);
    }
}
