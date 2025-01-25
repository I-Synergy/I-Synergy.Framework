using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Concurrent;
using static ISynergy.Framework.Core.Services.Tests.ScopedContextServiceTests;

namespace ISynergy.Framework.Core.Locators.Tests;

[TestClass]
public class ServiceLocatorTests
{
    #region Test interfaces and implementations
    public interface ITestScopedService
    {
        string InstanceId { get; }
    }

    public class TestScopedService : ITestScopedService
    {
        public string InstanceId { get; } = Guid.NewGuid().ToString();
    }

    public interface ITestSingletonService { }
    public class TestSingletonService : ITestSingletonService { }

    public interface ITestTransientService { }
    public class TestTransientService : ITestTransientService { }

    public interface INonExistentService { }

    public interface INestedScopeTestService
    {
        string ScopeId { get; }
    }
    public class NestedScopeTestService : INestedScopeTestService
    {
        public string ScopeId { get; } = Guid.NewGuid().ToString();
    }
    #endregion

    private IServiceProvider _serviceProvider;
    private ServiceLocator _locator;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Register test services
        services.AddScoped<ITestScopedService, TestScopedService>();
        services.AddSingleton<ITestSingletonService, TestSingletonService>();
        services.AddTransient<ITestTransientService, TestTransientService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<INestedScopeTestService, NestedScopeTestService>();

        _serviceProvider = services.BuildServiceProviderWithLocator(true);
        _locator = new ServiceLocator(_serviceProvider);
        ServiceLocator.SetLocatorProvider(_serviceProvider);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _locator?.Dispose();
    }

    [TestMethod]
    public void ServiceLocator_SetLocatorProvider_ShouldInitializeCorrectly()
    {
        // Act
        var locator = ServiceLocator.Default;

        // Assert
        Assert.IsNotNull(locator);
        Assert.IsNotNull(locator.ServiceProvider);
    }

    [TestMethod]
    public void ServiceLocator_Default_ReturnsSameInstance()
    {
        // Arrange - ensure ServiceLocator is initialized
        Assert.IsNotNull(_serviceProvider, "ServiceProvider should be initialized");

        // Act
        var instance1 = ServiceLocator.Default;
        var instance2 = ServiceLocator.Default;

        // Assert
        Assert.IsNotNull(instance1);
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void ServiceLocator_GetService_Generic_ShouldResolveService()
    {
        // Act
        var scopedService = _locator.GetService<ITestScopedService>();

        // Assert
        Assert.IsNotNull(scopedService);
        Assert.IsInstanceOfType(scopedService, typeof(TestScopedService));
    }

    [TestMethod]
    public void ServiceLocator_GetService_Type_ShouldResolveService()
    {
        // Act
        var scopedService = _locator.GetService(typeof(ITestScopedService));

        // Assert
        Assert.IsNotNull(scopedService);
        Assert.IsInstanceOfType(scopedService, typeof(TestScopedService));
    }

    [TestMethod]
    public void ServiceLocator_CreateNewScope_ShouldCreateNewInstances()
    {
        // Act
        var firstScopedService = _locator.GetService<ITestScopedService>();
        _locator.CreateNewScope();
        var secondScopedService = _locator.GetService<ITestScopedService>();

        // Assert
        Assert.AreNotSame(firstScopedService, secondScopedService);
    }

    [TestMethod]
    public void ServiceLocator_NestedScopes_ShouldMaintainCorrectHierarchy()
    {
        // Arrange
        var services = new List<INestedScopeTestService>();

        // Act
        services.Add(_locator.GetService<INestedScopeTestService>()); // Root scope

        _locator.CreateNewScope(); // Level 1
        services.Add(_locator.GetService<INestedScopeTestService>());

        _locator.CreateNewScope(); // Level 2
        services.Add(_locator.GetService<INestedScopeTestService>());

        // Assert
        Assert.AreEqual(3, services.Select(s => s.ScopeId).Distinct().Count(),
            "Each nested scope should have its own unique service instance");
        Assert.AreNotEqual(services[0].ScopeId, services[1].ScopeId);
        Assert.AreNotEqual(services[1].ScopeId, services[2].ScopeId);
    }

    [TestMethod]
    public async Task ServiceLocator_ConcurrentAccess_ShouldBeThreadSafe()
    {
        // Arrange
        int concurrentTasks = 10;
        var tasks = new List<Task<(ServiceLocator locator, string instanceId)>>();
        var instanceIds = new ConcurrentBag<string>();

        // Act
        for (int i = 0; i < concurrentTasks; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                // Create a new ServiceLocator for each task to ensure proper isolation
                var taskLocator = new ServiceLocator(_serviceProvider);
                var service = taskLocator.GetService<ITestScopedService>();
                var instanceId = service.InstanceId;
                instanceIds.Add(instanceId);
                return (taskLocator, instanceId);
            }));
        }

        var results = await Task.WhenAll(tasks);

        try
        {
            // Assert
            Assert.AreEqual(concurrentTasks, results.Length,
                "All concurrent requests should successfully retrieve a service");

            var uniqueInstanceIds = results.Select(r => r.instanceId).Distinct().ToList();

            // Diagnostic information
            Console.WriteLine($"Total results: {results.Length}");
            Console.WriteLine($"Unique instances: {uniqueInstanceIds.Count}");
            Console.WriteLine("Instance IDs:");
            foreach (var (_, id) in results)
            {
                Console.WriteLine($"  {id}");
            }

            Assert.AreEqual(concurrentTasks, uniqueInstanceIds.Count,
                "Each concurrent request should get a unique instance due to scoping");
        }
        finally
        {
            // Cleanup
            foreach (var (locator, _) in results)
            {
                locator?.Dispose();
            }
        }
    }

    [TestMethod]
    public void ServiceLocator_SingletonService_ShouldMaintainInstance()
    {
        // Act
        var firstSingletonService = _locator.GetService<ITestSingletonService>();
        _locator.CreateNewScope();
        var secondSingletonService = _locator.GetService<ITestSingletonService>();

        // Assert
        Assert.AreSame(firstSingletonService, secondSingletonService);
    }

    [TestMethod]
    public void ServiceLocator_TransientService_ShouldCreateNewInstancesEachTime()
    {
        // Act
        var firstTransientService = _locator.GetService<ITestTransientService>();
        var secondTransientService = _locator.GetService<ITestTransientService>();

        // Assert
        Assert.AreNotSame(firstTransientService, secondTransientService);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ServiceLocator_GetNonExistentService_ShouldThrowException()
    {
        _locator.GetService<INonExistentService>();
    }

    [TestMethod]
    public void ServiceLocator_Dispose_ShouldCleanupCorrectly()
    {
        // Arrange
        var localLocator = new ServiceLocator(_serviceProvider);

        // Act
        var scopedService = localLocator.GetService<ITestScopedService>();
        localLocator.Dispose();

        // Get a new instance after disposal
        var newLocator = new ServiceLocator(_serviceProvider);
        var newScopedService = newLocator.GetService<ITestScopedService>();

        // Assert
        Assert.IsNotNull(newScopedService);
        Assert.AreNotSame(scopedService, newScopedService);

        // Cleanup
        newLocator.Dispose();
    }

    [TestMethod]
    public void ServiceLocator_SetLocatorProvider_UpdatesDefaultInstance()
    {
        // Arrange
        var oldInstance = ServiceLocator.Default;
        var newServiceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        ServiceLocator.SetLocatorProvider(newServiceProvider);
        var newInstance = ServiceLocator.Default;

        // Assert
        Assert.AreNotSame(oldInstance, newInstance);
    }

    [TestMethod]
    public void ServiceLocator_CreateNewScope_NotifiesOfChange()
    {
        // Arrange
        var eventRaised = false;
        var eventArgs = false;

        // Act
        _locator.ScopedChanged += (s, e) =>
        {
            eventRaised = true;
            eventArgs = e.Value;
        };

        _locator.CreateNewScope();

        // Assert
        Assert.IsTrue(eventRaised);
        Assert.IsTrue(eventArgs);
    }

    [TestMethod]
    public void ServiceLocator_Dispose_CleansUpServices()
    {
        // Arrange
        var disposableService = new Mock<IDisposableTestService>();
        var services = new ServiceCollection();
        services.AddScoped<IDisposableTestService>(s => disposableService.Object);
        var provider = services.BuildServiceProvider();
        var locator = new ServiceLocator(provider);

        // Get service to ensure it's created
        var service = locator.GetService<IDisposableTestService>();

        // Act
        locator.Dispose();

        // Assert
        disposableService.Verify(s => s.Dispose(), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void ServiceLocator_GetServiceAfterDispose_ThrowsException()
    {
        // Arrange
        _locator.Dispose();

        // Act
        _locator.GetService<ITestService>();
    }
}