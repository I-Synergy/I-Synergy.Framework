using ISynergy.Framework.Core.Locators.Tests;
using ISynergy.Framework.Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static ISynergy.Framework.Core.Locators.Tests.ServiceLocatorTests;

namespace ISynergy.Framework.Core.Services.Tests;

[TestClass]
public class ScopedContextServiceTests
{
    private IServiceProvider _serviceProvider;
    private ScopedContextService _scopedContextService;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<ITestScopedService, TestScopedService>();
        services.AddScoped<INestedScopeTestService, ServiceLocatorTests.NestedScopeTestService>();
        _serviceProvider = services.BuildServiceProvider();
        _scopedContextService = new ScopedContextService(_serviceProvider);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _scopedContextService?.Dispose();
    }

    [TestMethod]
    public void ScopedContextService_GetService_ReturnsService()
    {
        // Act
        var service = _scopedContextService.GetService<ITestService>();

        // Assert
        Assert.IsNotNull(service);
        Assert.IsInstanceOfType(service, typeof(TestService));
    }

    [TestMethod]
    public void ScopedContextService_CreateNewScope_NotifiesOfChange()
    {
        // Arrange
        var eventRaised = false;
        var eventArgs = false;

        MessageService.Default.Register<ScopeChangedMessage>(this, m =>
        {
            eventRaised = true;
            eventArgs = m.Content;
        });

        // Act
        _scopedContextService.CreateNewScope();

        // Assert
        Assert.IsTrue(eventRaised);
        Assert.IsTrue(eventArgs);

        MessageService.Default.Unregister<ScopeChangedMessage>(this);
    }

    [TestMethod]
    public void ScopedContextService_NestedScopes_MaintainCorrectHierarchy()
    {
        // Arrange
        var services = new List<INestedScopeTestService>();

        // Act
        services.Add(_scopedContextService.GetService<INestedScopeTestService>()); // Root scope

        _scopedContextService.CreateNewScope(); // Level 1
        services.Add(_scopedContextService.GetService<INestedScopeTestService>());

        _scopedContextService.CreateNewScope(); // Level 2
        services.Add(_scopedContextService.GetService<INestedScopeTestService>());

        // Assert
        Assert.AreEqual(3, services.Select(s => s.ScopeId).Distinct().Count(),
            "Each nested scope should have its own unique service instance");
    }

    [TestMethod]
    public async Task ScopedContextService_ConcurrentAccess_ShouldBeThreadSafe()
    {
        // Arrange
        int concurrentTasks = 10;
        var tasks = new List<Task<(ScopedContextService service, string instanceId)>>();

        // Act
        for (int i = 0; i < concurrentTasks; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var scopedService = new ScopedContextService(_serviceProvider);
                var service = scopedService.GetService<ITestScopedService>();
                return (scopedService, service.InstanceId);
            }));
        }

        var results = await Task.WhenAll(tasks);

        try
        {
            // Assert
            var uniqueInstanceIds = results.Select(r => r.instanceId).Distinct().ToList();
            Assert.AreEqual(concurrentTasks, uniqueInstanceIds.Count,
                "Each concurrent request should get a unique instance");
        }
        finally
        {
            // Cleanup
            foreach (var (service, _) in results)
            {
                service?.Dispose();
            }
        }
    }

    [TestMethod]
    public void ScopedContextService_CreateNewScope_DisposesOldScope()
    {
        // Arrange
        var disposableService = new Mock<IDisposableTestService>();
        var services = new ServiceCollection();
        services.AddScoped<IDisposableTestService>(s => disposableService.Object);
        var provider = services.BuildServiceProvider();
        var scopedService = new ScopedContextService(provider);
        var oldService = scopedService.GetService<IDisposableTestService>();

        // Act
        scopedService.CreateNewScope();

        // Assert
        disposableService.Verify(s => s.Dispose(), Times.Once);

        // Cleanup
        scopedService.Dispose();
    }

    [TestMethod]
    public void ScopedContextService_Dispose_CleansUpServicesAndEvents()
    {
        // Arrange
        var disposableService = new Mock<IDisposableTestService>();
        var services = new ServiceCollection();
        services.AddScoped<IDisposableTestService>(_ => disposableService.Object);
        var provider = services.BuildServiceProvider();
        var scopedService = new ScopedContextService(provider);
        var service = scopedService.GetService<IDisposableTestService>();

        bool eventRaised = false;

        MessageService.Default.Register<ScopeChangedMessage>(this, m =>
        {
            eventRaised = true;
        });

        // Act
        scopedService.Dispose();

        // Assert
        disposableService.Verify(s => s.Dispose(), Times.Once);
        Assert.IsFalse(eventRaised);

        MessageService.Default.Unregister<ScopeChangedMessage>(this);
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void ScopedContextService_GetServiceAfterDispose_ThrowsException()
    {
        // Arrange
        _scopedContextService.Dispose();

        // Act
        _scopedContextService.GetService<ITestService>();
    }

    [TestMethod]
    public void ScopedContextService_ServiceProvider_ReturnsCurrentScope()
    {
        // Arrange & Act
        var provider = _scopedContextService.ServiceProvider;

        // Assert
        Assert.IsNotNull(provider);
        Assert.IsInstanceOfType(provider, typeof(IServiceProvider));
    }

    [TestMethod]
    public void ScopedContextService_MultipleDisposeCall_HandlesGracefully()
    {
        // Arrange
        var scopedService = new ScopedContextService(_serviceProvider);

        // Act & Assert - should not throw
        scopedService.Dispose();
        scopedService.Dispose(); // Second dispose should be handled gracefully
    }

    [TestMethod]
    public void ScopedContextService_EventHandlerCleanup_RemovesAllHandlers()
    {
        // Arrange
        var scopedService = new ScopedContextService(_serviceProvider);
        int eventCounter = 0;

        MessageService.Default.Register<ScopeChangedMessage>(this, m =>
        {
            eventCounter++;
        });

        // Act
        scopedService.CreateNewScope(); // Should trigger event
        Assert.AreEqual(1, eventCounter, "Event should have been handled");

        scopedService.Dispose(); // Should clean up event handlers

        // Try to create new scope after disposal - should throw but not trigger event
        try
        {
            scopedService.CreateNewScope();
        }
        catch (ObjectDisposedException)
        {
            // Expected exception
        }

        // Assert
        Assert.AreEqual(1, eventCounter, "Event should not have been triggered after disposal");

        MessageService.Default.Unregister<ScopeChangedMessage>(this);
    }

    // Test interfaces and classes
    public interface ITestService { }
    public interface IDisposableTestService : IDisposable { }
    public class TestService : ITestService { }
}