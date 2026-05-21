using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace ISynergy.Framework.Core.Locators;

[TestClass]
public class ServiceLocatorTests
{
    #region Test interfaces and implementations
    public interface ITestService { }
    public class TestService : ITestService { }

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

    public ServiceLocatorTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        services.AddSingleton<ITestSingletonService, TestSingletonService>();
        services.AddTransient<ITestTransientService, TestTransientService>();
        services.AddTransient<INestedScopeTestService, NestedScopeTestService>();

        _serviceProvider = services.BuildServiceProvider();
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
        var locator = ServiceLocator.Default;
        Assert.IsNotNull(locator);
        Assert.IsNotNull(locator.ServiceProvider);
    }

    [TestMethod]
    public void ServiceLocator_Default_ReturnsSameInstance()
    {
        Assert.IsNotNull(_serviceProvider, "ServiceProvider should be initialized");
        var instance1 = ServiceLocator.Default;
        var instance2 = ServiceLocator.Default;
        Assert.IsNotNull(instance1);
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void ServiceLocator_GetService_Generic_ShouldResolveService()
    {
        var service = _locator.GetRequiredService<ITestService>();
        Assert.IsNotNull(service);
        Assert.IsInstanceOfType(service, typeof(TestService));
    }

    [TestMethod]
    public void ServiceLocator_GetService_Type_ShouldResolveService()
    {
        var service = _locator.GetRequiredService(typeof(ITestService));
        Assert.IsNotNull(service);
        Assert.IsInstanceOfType(service, typeof(TestService));
    }

    [TestMethod]
    public async Task ServiceLocator_ConcurrentAccess_ShouldBeThreadSafe()
    {
        int concurrentTasks = 10;
        var tasks = new List<Task<string>>();
        var instanceIds = new ConcurrentBag<string>();

        for (int i = 0; i < concurrentTasks; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var taskLocator = new ServiceLocator(_serviceProvider);
                var service = taskLocator.GetRequiredService<ITestTransientService>();
                var id = service.GetHashCode().ToString();
                instanceIds.Add(id);
                taskLocator.Dispose();
                return id;
            }));
        }

        var results = await Task.WhenAll(tasks);

        Assert.AreEqual(concurrentTasks, results.Length,
            "All concurrent requests should successfully retrieve a service");
    }

    [TestMethod]
    public void ServiceLocator_SingletonService_ShouldMaintainInstance()
    {
        var first = _locator.GetRequiredService<ITestSingletonService>();
        var second = _locator.GetRequiredService<ITestSingletonService>();
        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void ServiceLocator_TransientService_ShouldCreateNewInstancesEachTime()
    {
        var first = _locator.GetRequiredService<ITestTransientService>();
        var second = _locator.GetRequiredService<ITestTransientService>();
        Assert.AreNotSame(first, second);
    }

    [TestMethod]
    public void ServiceLocator_GetNonExistentService_ShouldThrowException()
    {
        Assert.Throws<InvalidOperationException>(() => _locator.GetRequiredService<INonExistentService>());
    }

    [TestMethod]
    public void ServiceLocator_Dispose_ShouldCleanupCorrectly()
    {
        var localLocator = new ServiceLocator(_serviceProvider);
        var service = localLocator.GetRequiredService<ITestSingletonService>();
        localLocator.Dispose();

        var newLocator = new ServiceLocator(_serviceProvider);
        var newService = newLocator.GetRequiredService<ITestSingletonService>();

        Assert.IsNotNull(newService);
        newLocator.Dispose();
    }

    [TestMethod]
    public void ServiceLocator_SetLocatorProvider_UpdatesDefaultInstance()
    {
        var oldInstance = ServiceLocator.Default;
        var newServiceProvider = new ServiceCollection().BuildServiceProvider();
        ServiceLocator.SetLocatorProvider(newServiceProvider);
        var newInstance = ServiceLocator.Default;
        Assert.AreNotSame(oldInstance, newInstance);
    }

    [TestMethod]
    public void ServiceLocator_GetServiceAfterDispose_ThrowsException()
    {
        _locator.Dispose();
        Assert.Throws<ObjectDisposedException>(() => _locator.GetRequiredService<ITestService>());
    }
}
