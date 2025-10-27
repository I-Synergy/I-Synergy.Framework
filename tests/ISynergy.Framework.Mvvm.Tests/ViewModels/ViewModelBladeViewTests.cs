using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.ViewModels.Tests;

[TestClass]
public class ViewModelBladeViewTests
{
    private Mock<IScopedContextService> _mockScopedContextService;
    private Mock<ICommonServices> _mockCommonServices;
    private Mock<ILanguageService> _mockLanguageService;

    public ViewModelBladeViewTests()
    {
        _mockScopedContextService = new Mock<IScopedContextService>();

        _mockCommonServices = new Mock<ICommonServices>();
        _mockCommonServices.SetupGet(s => s.ScopedContextService).Returns(_mockScopedContextService.Object);

        _mockLanguageService = new Mock<ILanguageService>();
    }

    public class TestBladeViewModel : ViewModelBladeView<TestEntity>
    {
        public TestBladeViewModel(ICommonServices commonServices, ILogger<TestBladeViewModel> logger)
            : base(commonServices, logger)
        {
        }

        public override Task AddAsync()
        {
            return Task.CompletedTask;
        }

        public override Task EditAsync(TestEntity e)
        {
            return Task.CompletedTask;
        }

        public override Task RemoveAsync(TestEntity e)
        {
            return Task.CompletedTask;
        }

        public override Task SearchAsync(object e)
        {
            return Task.CompletedTask;
        }

        public override Task RetrieveItemsAsync(CancellationToken cancellationToken)
        {
            Items.Add(new TestEntity { Id = 1, Name = "Test" });
            return Task.CompletedTask;
        }
    }

    public class TestObservableBladeViewViewModel : ViewModelBladeView<TestObservableEntity>
    {
        public int PropertyChangedCallCount { get; private set; }

        public TestObservableBladeViewViewModel(ICommonServices commonServices, ILogger<TestObservableBladeViewViewModel> logger)
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

     public override Task AddAsync() => Task.CompletedTask;
        public override Task EditAsync(TestObservableEntity e) => Task.CompletedTask;
    public override Task RemoveAsync(TestObservableEntity e) => Task.CompletedTask;
        public override Task SearchAsync(object e) => Task.CompletedTask;
    }

    public class TestEntity
    {
     public int Id { get; set; }
 public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Observable entity for testing PropertyChanged subscription
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
    public void Constructor_InitializesCollectionsAndCommands()
    {
        // Arrange & Act
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

 // Assert
        Assert.IsNotNull(viewModel.Items);
      Assert.IsNotNull(viewModel.Blades);
Assert.IsNotNull(viewModel.AddCommand);
     Assert.IsNotNull(viewModel.EditCommand);
  Assert.IsNotNull(viewModel.DeleteCommand);
        Assert.IsNotNull(viewModel.RefreshCommand);
        Assert.IsNotNull(viewModel.SearchCommand);
        Assert.IsNotNull(viewModel.SubmitCommand);
  Assert.IsFalse(viewModel.IsPaneVisible);
    }

    [TestMethod]
    public async Task Initialize_WithRefreshOnInitialization_CallsRefresh()
    {
        // Arrange
      var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

     // Act
      await viewModel.InitializeAsync();

   // Assert
        Assert.IsTrue(viewModel.IsInitialized);
    }

    [TestMethod]
    public void SetSelectedItem_UpdatesPropertyAndFlag()
 {
     // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
        var entity = new TestEntity { Id = 1, Description = "Test" };

        // Act
        viewModel.SetSelectedItem(entity);

  // Assert
   Assert.AreEqual(entity, viewModel.SelectedItem);
     Assert.IsTrue(viewModel.IsUpdate);
    }

    [TestMethod]
    public void Cleanup_ClearsCollectionsAndCommands()
    {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);
     viewModel.Items.Add(new TestEntity());

        // Act
   viewModel.Cleanup();
     Assert.AreEqual(0, viewModel.Items.Count);

        viewModel.Dispose();
        Assert.IsTrue(viewModel.IsDisposed);
    }

    [TestMethod]
 public async Task RetrieveItemsAsync_PopulatesItems()
 {
        // Arrange
        var viewModel = new TestBladeViewModel(_mockCommonServices.Object, new Mock<ILogger<TestBladeViewModel>>().Object);

        // Act
        await viewModel.RetrieveItemsAsync(CancellationToken.None);

   // Assert
   Assert.AreEqual(1, viewModel.Items.Count);
    }

  #region PropertyChanged Subscription Tests

    [TestMethod]
    public void SelectedItem_WhenSetWithObservableEntity_SubscribesToPropertyChanged()
    {
        // Arrange
        var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
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
        var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
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
        var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
 var firstEntity = new TestObservableEntity { Name = "First" };
        var secondEntity = new TestObservableEntity { Name = "Second" };

   // Set first entity and establish baseline
 viewModel.SetSelectedItem(firstEntity);
        var baselineCount = viewModel.PropertyChangedCallCount;

        // Act - Replace with second entity (this will increment count once)
   viewModel.SetSelectedItem(secondEntity);
  var countAfterReplacement = viewModel.PropertyChangedCallCount;

 // Change the FIRST entity (should NOT trigger PropertyChanged)
        firstEntity.Name = "Changed";
        var countAfterFirstChange = viewModel.PropertyChangedCallCount;

   // Change the SECOND entity (should trigger PropertyChanged)
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
    public void SubmitCommand_WhenObservableEntityPropertyChanges_NotifyCanExecuteChangedIsCalled()
    {
  // Arrange
   var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
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
   var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
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
   var viewModel = new TestObservableBladeViewViewModel(_mockCommonServices.Object, new Mock<ILogger<TestObservableBladeViewViewModel>>().Object);
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
}
