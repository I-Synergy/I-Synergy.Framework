using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelDialogTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILogger> _mockLogger;

    public ViewModelDialogTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLogger = new Mock<ILogger>();
    }
    public class TestDialogViewModel : ViewModelDialog<TestEntity>
    {
        public TestDialogViewModel(ICommonServices commonServices, ILogger<TestDialogViewModel> logger)
            : base(commonServices, logger) { }
    }

    public class TestObservableDialogViewModel : ViewModelDialog<TestObservableEntity>
    {
        public int PropertyChangedCallCount { get; private set; }

        public TestObservableDialogViewModel(ICommonServices commonServices, ILogger<TestObservableDialogViewModel> logger)
    : base(commonServices, logger)
        {
            PropertyChanged += (s, e) =>
     {
         if (e.PropertyName == nameof(SelectedItem))
         {
             PropertyChangedCallCount++;
         }
     };
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Observable entity similar to ThemeStyle
    /// </summary>
    public class TestObservableEntity : ObservableClass
    {
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public int Value
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
    }

    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        // Arrange & Act
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);

        // Assert
        Assert.IsNotNull(viewModel.SubmitCommand);
        Assert.IsFalse(viewModel.IsUpdate);
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public async Task SubmitAsync_WithValidation_InvokesSubmittedAndClosedEvents()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var submittedInvoked = false;
        var closedInvoked = false;
        viewModel.Submitted += (s, e) => submittedInvoked = true;
        viewModel.Closed += (s, e) => closedInvoked = true;

        // Act
        await viewModel.SubmitAsync(entity);

        // Assert
        Assert.IsTrue(submittedInvoked);
        Assert.IsTrue(closedInvoked);
    }

    [TestMethod]
    public void ApplyQueryAttributes_SetsSelectedItem()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var query = new Dictionary<string, object>
  {
              { GenericConstants.Parameter, entity }
    };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.AreEqual(entity, viewModel.SelectedItem);
        Assert.IsTrue(viewModel.IsUpdate);
    }

    [TestMethod]
    public void ApplyQueryAttributes_WithInvalidParameter_DoesNotSetSelectedItem()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var query = new Dictionary<string, object>
      {
         { "wrongKey", new TestEntity() }
   };

        // Act
        viewModel.ApplyQueryAttributes(query);

        // Assert
        Assert.IsNull(viewModel.SelectedItem);
        Assert.IsFalse(viewModel.IsUpdate);
    }

    [TestMethod]
    public void Cleanup_ClearsSelectedItemAndCommands()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        viewModel.SetSelectedItem(new TestEntity());

        // Act
        viewModel.Cleanup();
        Assert.IsNull(viewModel.SelectedItem);

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }

    #region PropertyChanged Subscription Tests

    [TestMethod]
    public void SelectedItem_WhenSetWithObservableEntity_SubscribesToPropertyChanged()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Initial", Value = 10 };

        // Act
        viewModel.SetSelectedItem(entity);
        var initialCount = viewModel.PropertyChangedCallCount;

        // Change a property on the entity
        entity.Name = "Changed";

        // Assert
        Assert.IsTrue(viewModel.PropertyChangedCallCount > initialCount,
            "PropertyChanged should be raised when nested property changes");
    }

    [TestMethod]
    public void SelectedItem_WhenNestedPropertyChanges_RaisesPropertyChangedForSelectedItem()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test", Value = 5 };
        viewModel.SetSelectedItem(entity);

        var propertyChangedRaised = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedItem))
                propertyChangedRaised = true;
        };

        // Act - Change nested property
        entity.Value = 20;

        // Assert
        Assert.IsTrue(propertyChangedRaised,
        "SelectedItem PropertyChanged should be raised when nested property changes");
    }

    [TestMethod]
    public void SelectedItem_WhenReplacedWithNewObservableEntity_UnsubscribesFromOld()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var firstEntity = new TestObservableEntity { Name = "First" };
        var secondEntity = new TestObservableEntity { Name = "Second" };

        // Set first entity and establish baseline
        viewModel.SetSelectedItem(firstEntity);
      var baselineCount = viewModel.PropertyChangedCallCount;

        // Act - Replace with second entity (this will increment count once for SelectedItem change)
        viewModel.SetSelectedItem(secondEntity);
        var countAfterReplacement = viewModel.PropertyChangedCallCount;

        // Change the FIRST entity (should NOT trigger PropertyChanged because we unsubscribed)
        firstEntity.Name = "Changed";
        var countAfterFirstChange = viewModel.PropertyChangedCallCount;

        // Change the SECOND entity (should trigger PropertyChanged because we're subscribed)
        secondEntity.Name = "Changed";
        var countAfterSecondChange = viewModel.PropertyChangedCallCount;

// Assert
        Assert.AreEqual(countAfterReplacement, countAfterFirstChange,
            "Changing old entity should not raise PropertyChanged after replacement");
      Assert.IsTrue(countAfterSecondChange > countAfterFirstChange,
    "Changing new entity should raise PropertyChanged");
        Assert.AreEqual(baselineCount + 1, countAfterReplacement,
            "Replacing SelectedItem should raise PropertyChanged once");
    }

    [TestMethod]
    public void SelectedItem_WhenSetToNull_UnsubscribesFromPrevious()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test" };
        viewModel.SetSelectedItem(entity);
        var countAfterSet = viewModel.PropertyChangedCallCount;

        // Act - Set to null (this will increment PropertyChangedCallCount once for SelectedItem changing)
        viewModel.SelectedItem = null;
        var countAfterNull = viewModel.PropertyChangedCallCount;

        // Change the entity (should NOT trigger PropertyChanged because we unsubscribed)
        entity.Name = "Changed";
        var countAfterChange = viewModel.PropertyChangedCallCount;

        // Assert
        Assert.AreEqual(countAfterNull, countAfterChange,
      "Changing entity after setting SelectedItem to null should not raise PropertyChanged");
        Assert.AreEqual(countAfterSet + 1, countAfterNull,
            "Setting SelectedItem to null should raise PropertyChanged once");
    }

    [TestMethod]
    public void SelectedItem_WithNonObservableEntity_DoesNotSubscribe()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var entity = new TestEntity { Name = "Test" };

        // Act
        viewModel.SetSelectedItem(entity);

        // No exception should be thrown, and command should work
        Assert.IsNotNull(viewModel.SelectedItem);
        Assert.IsTrue(viewModel.SubmitCommand.CanExecute(entity));
    }

    #endregion

    #region Command Re-evaluation Tests

    [TestMethod]
    public void SubmitCommand_WhenSelectedItemIsNull_CannotExecute()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);

        // Act & Assert
        Assert.IsFalse(viewModel.SubmitCommand.CanExecute(null));
    }

    [TestMethod]
    public void SubmitCommand_WhenSelectedItemIsNotNull_CanExecute()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var entity = new TestEntity { Name = "Test" };

        // Act
        viewModel.SetSelectedItem(entity);

        // Assert
        Assert.IsTrue(viewModel.SubmitCommand.CanExecute(entity));
    }

    [TestMethod]
    public void SubmitCommand_WhenObservableEntityPropertyChanges_CanExecuteRemainsTrue()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test", Value = 10 };
        viewModel.SetSelectedItem(entity);

        var canExecuteBefore = viewModel.SubmitCommand.CanExecute(entity);

        // Act - Change nested property
        entity.Value = 20;

        var canExecuteAfter = viewModel.SubmitCommand.CanExecute(entity);

        // Assert
        Assert.IsTrue(canExecuteBefore, "Command should be executable before property change");
        Assert.IsTrue(canExecuteAfter, "Command should remain executable after property change");
    }

    [TestMethod]
    public void SubmitCommand_WhenSelectedItemSet_NotifyCanExecuteChangedIsCalled()
    {
        // Arrange
        var viewModel = new TestDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestDialogViewModel>>().Object);
        var entity = new TestEntity { Name = "Test" };
        var canExecuteChangedRaised = false;

        viewModel.SubmitCommand.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        // Act
        viewModel.SetSelectedItem(entity);

        // Assert
        Assert.IsTrue(canExecuteChangedRaised, "CanExecuteChanged should be raised when SelectedItem is set");
    }

    [TestMethod]
    public void SubmitCommand_WhenObservableEntityPropertyChanges_NotifyCanExecuteChangedIsCalled()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test", Value = 10 };
        viewModel.SetSelectedItem(entity);

        var canExecuteChangedRaised = false;
        viewModel.SubmitCommand.CanExecuteChanged += (s, e) => canExecuteChangedRaised = true;

        // Act
        entity.Value = 20;

        // Assert
        Assert.IsTrue(canExecuteChangedRaised,
         "CanExecuteChanged should be raised when nested observable property changes");
    }

    #endregion

    #region Cleanup and Disposal Tests

    [TestMethod]
    public void Cleanup_WithObservableEntity_UnsubscribesFromPropertyChanged()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test" };
        viewModel.SetSelectedItem(entity);
        var countBeforeCleanup = viewModel.PropertyChangedCallCount;

        // Act
        viewModel.Cleanup();

        // Change entity after cleanup
        entity.Name = "Changed";
        var countAfterCleanup = viewModel.PropertyChangedCallCount;

        // Assert
        Assert.AreEqual(countBeforeCleanup, countAfterCleanup,
            "PropertyChanged should not be raised after cleanup");
        Assert.IsNull(viewModel.SelectedItem);
    }

    [TestMethod]
    public void Dispose_WithObservableEntity_UnsubscribesFromPropertyChanged()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Test" };
        viewModel.SetSelectedItem(entity);
        var countBeforeDispose = viewModel.PropertyChangedCallCount;

        // Act
        viewModel.Dispose();

        // Change entity after disposal
        entity.Name = "Changed";
        var countAfterDispose = viewModel.PropertyChangedCallCount;

        // Assert
        Assert.AreEqual(countBeforeDispose, countAfterDispose,
        "PropertyChanged should not be raised after disposal");
    }

    #endregion

    #region ThemeStyle-like Scenario Tests

    [TestMethod]
    public void ThemeStyleScenario_WhenMultiplePropertiesChange_RaisesPropertyChangedForEach()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Light", Value = 1 };
        viewModel.SetSelectedItem(entity);

        var propertyChangeCount = 0;
        viewModel.PropertyChanged += (s, e) =>
{
    if (e.PropertyName == nameof(viewModel.SelectedItem))
        propertyChangeCount++;
};

        // Act - Simulate user changing theme and color like in ThemeWindow
        entity.Name = "Dark";  // Theme change
        entity.Value = 2;      // Color change

        // Assert
        Assert.AreEqual(2, propertyChangeCount,
       "PropertyChanged should be raised twice for two property changes");
    }

    [TestMethod]
    public void ThemeStyleScenario_CommandCanExecute_AlwaysTrueAfterInitialization()
    {
        // Arrange
        var viewModel = new TestObservableDialogViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableDialogViewModel>>().Object);
        var entity = new TestObservableEntity { Name = "Light", Value = 1 };

        // Act
        viewModel.SetSelectedItem(entity);

        // Assert - Command should be executable immediately
        Assert.IsTrue(viewModel.SubmitCommand.CanExecute(viewModel.SelectedItem),
            "SubmitCommand should be executable when SelectedItem is not null");

        // Act - Change properties
        entity.Name = "Dark";
        entity.Value = 2;

        // Assert - Command should still be executable
        Assert.IsTrue(viewModel.SubmitCommand.CanExecute(viewModel.SelectedItem),
                    "SubmitCommand should remain executable after property changes");
    }

    #endregion
}
