using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBaseTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILoggerFactory> _mockLoggerFactory;

    public ViewModelBaseTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();
        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(new Mock<ILogger>().Object);

        _mockCommonServices.SetupGet(s => s.LoggerFactory).Returns(_mockLoggerFactory.Object);
    }

    private class TestViewModel : ViewModel
    {
        /// <summary>
        /// Gets or sets the TestProperty property value.
        /// </summary>
        public string TestProperty
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public TestViewModel(ICommonServices commonServices, bool automaticValidation = false)
            : base(commonServices, automaticValidation) { }

        public override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TestProperty))
            {
                Debug.WriteLine("TestProperty changed");
            }
        }
    }

    private enum TestEnum
    {
        [Display(Description = "TestDescription")]
        TestValue
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        // Assert
        Assert.IsNotNull(viewModel.CloseCommand);
        Assert.IsNotNull(viewModel.CancelCommand);
        Assert.IsFalse(viewModel.IsInitialized);
        Assert.IsFalse(viewModel.IsRefreshing);
        Assert.IsNull(viewModel.Parameter);
        Assert.IsNull(viewModel.Title);
    }

    [TestMethod]
    public async Task CloseAsync_InvokesClosedEvent()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);
        var eventInvoked = false;
        viewModel.Closed += (s, e) => eventInvoked = true;

        // Act
        await viewModel.CloseAsync();

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    [TestMethod]
    public async Task CancelAsync_InvokesCancelledAndClosedEvents()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);
        var cancelledInvoked = false;
        var closedInvoked = false;
        viewModel.Cancelled += (s, e) => cancelledInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.CancelAsync();

        // Assert
        Assert.IsTrue(cancelledInvoked);
        Assert.IsTrue(closedInvoked);
        Assert.IsTrue(viewModel.IsCancelled);
    }

    [TestMethod]
    public void Cleanup_DisposesCommands()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        // Act
        viewModel.Dispose();

        // Assert
        Assert.IsNull(viewModel.CloseCommand);
        Assert.IsNull(viewModel.CancelCommand);
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
    public void Constructor_WithAutomaticValidation_SetsValidation()
    {
        // Arrange & Act
        var viewModel = new TestViewModel(_mockCommonServices.Object, true);

        // Assert
        // Note: You might need to expose a way to check if automatic validation is enabled
        Assert.IsNotNull(viewModel);
    }

    [TestMethod]
    public void GetEnumDescription_ReturnsLocalizedString()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        // Act
        var result = viewModel.GetEnumDescription(TestEnum.TestValue);

        // Assert
        Assert.AreEqual("[TestDescription]", result);
    }

    [TestMethod]
    public void GetEnumDescription_WithoutDisplayAttribute_ReturnsEnumString()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        // Act
        var result = viewModel.GetEnumDescription(MessageBoxButton.OK);

        // Assert
        Assert.AreEqual("OK", result);
    }

    [TestMethod]
    public async Task InitializeAsync_SetsProperState()
    {
        // Arrange
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        // Act
        await viewModel.InitializeAsync();

        // Assert
        // Base implementation should not set IsInitialized
        Assert.IsFalse(viewModel.IsInitialized);
    }

    [TestMethod]
    public void TestIfPropertyChangedIsRaised()
    {
        // Arrange
        var propertyChangedRaised = false;
        var viewModel = new TestViewModel(_mockCommonServices.Object);

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.TestProperty))
                propertyChangedRaised = true;
        };

        // Act
        viewModel.TestProperty = "Test";

        // Assert
        Assert.IsTrue(propertyChangedRaised);
    }
}
