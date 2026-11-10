using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.UI.Services.Base;

/// <summary>
/// Tests for <see cref="BaseBladeService"/>: scope creation/disposal, callback wiring, blade
/// lifecycle (add/remove/pane state) and the exception handling helpers.
/// </summary>
[TestClass]
public class BaseBladeServiceTests
{
    private const string CallbackFailure = "callback failed";

    private Mock<IExceptionHandlerService> _exceptionHandler = null!;
    private Mock<ILogger> _logger = null!;
    private Mock<IViewModelBladeView> _owner = null!;
    private Mock<IViewModelBlade<TestEntity>> _blade = null!;
    private Mock<IView> _view = null!;
    private Mock<IServiceScope> _scope = null!;
    private Mock<IServiceScopeFactory> _scopeFactory = null!;
    private Dictionary<Type, object?> _registrations = null!;
    private TestBladeService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _exceptionHandler = new Mock<IExceptionHandlerService>();
        _logger = new Mock<ILogger>();

        _owner = new Mock<IViewModelBladeView>();
        _owner.SetupProperty(x => x.Blades, []);
        _owner.SetupProperty(x => x.IsPaneVisible);

        _blade = new Mock<IViewModelBlade<TestEntity>>();
        _blade.SetupProperty(x => x.Owner!);
        _blade.SetupProperty(x => x.IsInitialized);
        _blade.SetupProperty(x => x.SelectedItem);

        _view = new Mock<IView>();
        _view.SetupProperty(x => x.ViewModel!);
        _view.SetupProperty(x => x.IsEnabled);

        _registrations = new Dictionary<Type, object?>
        {
            [typeof(IViewModelBlade<TestEntity>)] = _blade.Object,
            [typeof(TestView)] = _view.Object
        };

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns((Type serviceType) =>
                _registrations.TryGetValue(serviceType, out var instance) ? instance : null);

        _scope = new Mock<IServiceScope>();
        _scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        _scopeFactory = new Mock<IServiceScopeFactory>();
        _scopeFactory.Setup(x => x.CreateScope()).Returns(_scope.Object);

        _service = new TestBladeService(
            _scopeFactory.Object,
            _exceptionHandler.Object,
            _logger.Object,
            typeof(TestView));
    }

    #region Constructor

    [TestMethod]
    public void Constructor_WithNullScopeFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestBladeService(null!, _exceptionHandler.Object, _logger.Object, typeof(TestView)));
    }

    [TestMethod]
    public void Constructor_WithNullExceptionHandler_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestBladeService(_scopeFactory.Object, null!, _logger.Object, typeof(TestView)));
    }

    [TestMethod]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestBladeService(_scopeFactory.Object, _exceptionHandler.Object, null!, typeof(TestView)));
    }

    #endregion

    #region OpenBladeAsync

    [TestMethod]
    public async Task OpenBladeAsync_WithEntity_OpensBladeAndWiresOwner()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, entity);

        // Assert
        _blade.Verify(x => x.SetSelectedItem(entity), Times.Once);
        _blade.Verify(x => x.InitializeAsync(), Times.Once);
        Assert.AreSame(_owner.Object, _blade.Object.Owner);
        Assert.AreSame(_blade.Object, _view.Object.ViewModel);
        Assert.IsTrue(_view.Object.IsEnabled);
        Assert.HasCount(1, _owner.Object.Blades!);
        Assert.AreSame(_view.Object, _owner.Object.Blades![0]);
        Assert.IsTrue(_owner.Object.IsPaneVisible);
        Assert.IsTrue(_service.MonitorViewLoadCalled);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WithoutEntity_DoesNotSetSelectedItem()
    {
        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object);

        // Assert
        _blade.Verify(x => x.SetSelectedItem(It.IsAny<TestEntity>()), Times.Never);
        Assert.HasCount(1, _owner.Object.Blades!);
    }

    [TestMethod]
    public async Task OpenBladeAsync_InvokesInitializationCallback()
    {
        // Arrange
        IViewModelBlade<TestEntity>? initialized = null;

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            initialization: vm => initialized = vm);

        // Assert
        Assert.AreSame(_blade.Object, initialized);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenViewModelAlreadyInitialized_SkipsInitialize()
    {
        // Arrange
        _blade.Object.IsInitialized = true;

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity());

        // Assert
        _blade.Verify(x => x.InitializeAsync(), Times.Never);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenResolvedServiceIsNotABladeViewModel_DisposesScopeAndReturns()
    {
        // Arrange
        _registrations[typeof(IViewModelBlade<TestEntity>)] = new object();

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity());

        // Assert
        _scope.Verify(x => x.Dispose(), Times.Once);
        Assert.HasCount(0, _owner.Object.Blades!);
        Assert.IsFalse(_service.MonitorViewLoadCalled);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenViewTypeCannotBeResolved_ThrowsAndDisposesScope()
    {
        // Arrange
        _service = new TestBladeService(
            _scopeFactory.Object,
            _exceptionHandler.Object,
            _logger.Object,
            viewType: null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity()));

        _scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenResolvedViewIsNotRegistered_ThrowsAndDisposesScope()
    {
        // Arrange - the view type is known but the scope cannot provide it
        _registrations.Remove(typeof(TestView));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity()));

        _scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenOwnerNavigatedFromThrows_HandlesException()
    {
        // Arrange
        _owner.Setup(x => x.OnNavigatedFrom()).Throws(new InvalidOperationException("navigated from failed"));

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity());

        // Assert
        _exceptionHandler.Verify(x => x.HandleException(It.IsAny<InvalidOperationException>()), Times.Once);
        Assert.HasCount(1, _owner.Object.Blades!);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WithOwnerHostTarget_UsesResolvedHost()
    {
        // Arrange
        var host = new Mock<IViewModelBladeView>();
        host.SetupProperty(x => x.Blades, []);
        host.SetupProperty(x => x.IsPaneVisible);
        _service.HostOverride = host.Object;

        // Act
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            hostTarget: BladeHostTargets.Owner);

        // Assert
        CollectionAssert.Contains(_service.HostTargetsSeen, BladeHostTargets.Owner);
        Assert.HasCount(1, host.Object.Blades!);
        Assert.HasCount(0, _owner.Object.Blades!);
    }

    #endregion

    #region Callbacks

    [TestMethod]
    public async Task OpenBladeAsync_WhenBladeSubmits_InvokesSubmittedCallback()
    {
        // Arrange
        var entity = new TestEntity();
        var submitted = new TaskCompletionSource<TestEntity>(TaskCreationOptions.RunContinuationsAsynchronously);

        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            entity,
            submittedEventHandler: result =>
            {
                submitted.TrySetResult(result);
                return Task.CompletedTask;
            });

        // Act
        _blade.Raise(x => x.Submitted += null, new SubmitEventArgs<TestEntity>(entity));

        // Assert
        var received = await submitted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.AreSame(entity, received);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenSubmittedCallbackThrows_LogsError()
    {
        // Arrange
        var submitted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            submittedEventHandler: _ =>
            {
                submitted.TrySetResult();
                throw new InvalidOperationException(CallbackFailure);
            });

        // Act
        _blade.Raise(x => x.Submitted += null, new SubmitEventArgs<TestEntity>(new TestEntity()));
        await submitted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert
        VerifyErrorLogged(CallbackFailure);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenBladeCancels_InvokesCancelledCallback()
    {
        // Arrange
        var cancelled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            cancelledEventHandler: () =>
            {
                cancelled.TrySetResult();
                return Task.CompletedTask;
            });

        // Act
        _blade.Raise(x => x.Cancelled += null, EventArgs.Empty);

        // Assert
        await cancelled.Task.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenBladeClosed_InvokesClosedCallbackAndDisposesScope()
    {
        // Arrange
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            closedEventHandler: () =>
            {
                closed.TrySetResult();
                return Task.CompletedTask;
            });

        // Act
        _blade.Raise(x => x.Closed += null, EventArgs.Empty);

        // Assert
        await closed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        _scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenClosedCallbackThrows_LogsErrorAndStillDisposesScope()
    {
        // Arrange
        var invoked = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(
            _owner.Object,
            new TestEntity(),
            closedEventHandler: () =>
            {
                invoked.TrySetResult();
                throw new InvalidOperationException(CallbackFailure);
            });

        // Act
        _blade.Raise(x => x.Closed += null, EventArgs.Empty);
        await invoked.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // Assert
        VerifyErrorLogged(CallbackFailure);
        _scope.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task OpenBladeAsync_WhenBladeClosed_RemovesBladeFromOwner()
    {
        // Arrange
        await _service.OpenBladeAsync<IViewModelBlade<TestEntity>, TestEntity>(_owner.Object, new TestEntity());

        // Act - the auto-removal handler registered by PrepareOwnerForBlade runs on Closed
        _blade.Raise(x => x.Closed += null, EventArgs.Empty);

        // Assert
        Assert.HasCount(0, _owner.Object.Blades!);
        Assert.IsFalse(_owner.Object.IsPaneVisible);
        Assert.AreSame(_view.Object, _service.DetachedView);
    }

    #endregion

    #region RemoveBlade

    [TestMethod]
    public void RemoveBlade_WithNonBladeViewModel_DoesNothing()
    {
        // Arrange
        var blades = new ObservableCollection<IView> { _view.Object };
        _owner.Object.Blades = blades;

        // Act
        _service.RemoveBlade(_owner.Object, Mock.Of<IViewModel>());

        // Assert
        Assert.HasCount(1, blades);
        Assert.IsNull(_service.DetachedView);
    }

    [TestMethod]
    public void RemoveBlade_WhenBladesCollectionIsNull_DoesNothing()
    {
        // Arrange
        _owner.Object.Blades = null;

        // Act
        _service.RemoveBlade(_owner.Object, _blade.Object);

        // Assert - no exception and the view model was still navigated from
        _blade.Verify(x => x.OnNavigatedFrom(), Times.Once);
        Assert.IsNull(_service.DetachedView);
    }

    [TestMethod]
    public void RemoveBlade_WhenBladePresent_DetachesViewAndHidesPane()
    {
        // Arrange
        _blade.Object.Owner = _owner.Object;
        var blades = new ObservableCollection<IView> { _view.Object };
        _view.Object.ViewModel = _blade.Object;
        _owner.Object.Blades = blades;
        _owner.Object.IsPaneVisible = true;

        // Act
        _service.RemoveBlade(_owner.Object, _blade.Object);

        // Assert
        Assert.HasCount(0, blades);
        Assert.IsFalse(_owner.Object.IsPaneVisible);
        Assert.AreSame(_view.Object, _service.DetachedView);
    }

    [TestMethod]
    public void RemoveBlade_WhenAnotherBladeRemains_EnablesAndNavigatesToTopmostBlade()
    {
        // Arrange
        var otherBlade = new Mock<IViewModelBlade<TestEntity>>();

        _blade.Object.Owner = _owner.Object;
        var removedView = new FakeView { ViewModel = _blade.Object };
        var remainingView = new FakeView { ViewModel = otherBlade.Object };
        var blades = new ObservableCollection<IView> { removedView, remainingView };
        _owner.Object.Blades = blades;

        // Act
        _service.RemoveBlade(_owner.Object, _blade.Object);

        // Assert
        Assert.HasCount(1, blades);
        Assert.AreSame(remainingView, blades[0]);
        Assert.IsTrue(remainingView.IsEnabled);
        otherBlade.Verify(x => x.OnNavigatedTo(), Times.Once);
        Assert.AreSame(removedView, _service.DetachedView);
    }

    [TestMethod]
    public void RemoveBlade_WhenNoBladeMatches_LeavesOwnerUnchanged()
    {
        // Arrange - the view in the collection belongs to another owner
        var otherBlade = new Mock<IViewModelBlade<TestEntity>>();
        otherBlade.SetupProperty(x => x.Owner!);
        otherBlade.Object.Owner = Mock.Of<IViewModelBladeView>();

        _blade.Object.Owner = _owner.Object;
        var blades = new ObservableCollection<IView> { new FakeView { ViewModel = otherBlade.Object } };
        _owner.Object.Blades = blades;

        // Act
        _service.RemoveBlade(_owner.Object, _blade.Object);

        // Assert
        Assert.HasCount(1, blades);
        Assert.IsNull(_service.DetachedView);
    }

    #endregion

    #region Helpers

    private void VerifyErrorLogged(string message) =>
        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state!.ToString()!.Contains(message, StringComparison.Ordinal)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

    public sealed class TestEntity
    {
    }

    public sealed class TestView
    {
    }

    private sealed class FakeView : IView
    {
        public IViewModel ViewModel { get; set; } = null!;
        public bool IsEnabled { get; set; }
        public void Dispose()
        {
        }
    }

    private sealed class TestBladeService : BaseBladeService
    {
        private readonly Type? _viewType;

        public TestBladeService(
            IServiceScopeFactory scopeFactory,
            IExceptionHandlerService exceptionHandlerService,
            ILogger logger,
            Type? viewType)
            : base(scopeFactory, exceptionHandlerService, logger)
        {
            _viewType = viewType;
        }

        public IView? DetachedView { get; private set; }

        public bool MonitorViewLoadCalled { get; private set; }

        public IViewModelBladeView? HostOverride { get; set; }

        public List<BladeHostTargets> HostTargetsSeen { get; } = [];

        protected override Type? ResolveViewType(IViewModel bladeVm) => _viewType;

        protected override Task MonitorViewLoadAsync(IView view, int timeoutMs = 500)
        {
            MonitorViewLoadCalled = true;
            return Task.CompletedTask;
        }

        protected override void DetachBladeView(IView view) => DetachedView = view;

        protected override IViewModelBladeView ResolveBladeHost(IViewModelBladeView owner, BladeHostTargets hostTarget)
        {
            HostTargetsSeen.Add(hostTarget);
            return HostOverride ?? owner;
        }
    }

    #endregion
}
