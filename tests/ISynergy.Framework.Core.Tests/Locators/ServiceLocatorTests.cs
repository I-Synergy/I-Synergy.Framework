using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Locators.Tests;

[TestClass]
public class ServiceLocatorTests
{
    #region Test interfaces and implementations
    public interface ITestScopedService { }
    public class TestScopedService : ITestScopedService { }

    public interface ITestSingletonService { }
    public class TestSingletonService : ITestSingletonService { }

    public interface ITestTransientService { }
    public class TestTransientService : ITestTransientService { }

    public interface INonExistentService { }
    #endregion

    private IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Register test services
        services.AddScoped<ITestScopedService, TestScopedService>();
        services.AddSingleton<ITestSingletonService, TestSingletonService>();
        services.AddTransient<ITestTransientService, TestTransientService>();

        return services.BuildServiceProviderWithLocator(true);
    }

    [TestMethod]
    public void ServiceLocator_SetLocatorProvider_ShouldInitializeCorrectly()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();

        // Act
        var locator = ServiceLocator.Default;

        // Assert
        Assert.IsNotNull(locator);
        Assert.IsNotNull(locator.ServiceProvider);
    }

    [TestMethod]
    public void ServiceLocator_GetInstance_Generic_ShouldResolveService()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var scopedService = locator.GetService<ITestScopedService>();

        // Assert
        Assert.IsNotNull(scopedService);
        Assert.IsInstanceOfType(scopedService, typeof(TestScopedService));
    }

    [TestMethod]
    public void ServiceLocator_GetInstance_Type_ShouldResolveService()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var scopedService = locator.GetService(typeof(ITestScopedService));

        // Assert
        Assert.IsNotNull(scopedService);
        Assert.IsInstanceOfType(scopedService, typeof(TestScopedService));
    }

    [TestMethod]
    public void ServiceLocator_CreateNewScope_ShouldCreateNewInstances()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var firstScopedService = locator.GetService<ITestScopedService>();
        locator.CreateNewScope();
        var secondScopedService = locator.GetService<ITestScopedService>();

        // Assert
        Assert.AreNotSame(firstScopedService, secondScopedService);
    }

    [TestMethod]
    public void ServiceLocator_SingletonService_ShouldMaintainInstance()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var firstSingletonService = locator.GetService<ITestSingletonService>();
        locator.CreateNewScope();
        var secondSingletonService = locator.GetService<ITestSingletonService>();

        // Assert
        Assert.AreSame(firstSingletonService, secondSingletonService);
    }

    [TestMethod]
    public void ServiceLocator_TransientService_ShouldCreateNewInstancesEachTime()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var firstTransientService = locator.GetService<ITestTransientService>();
        var secondTransientService = locator.GetService<ITestTransientService>();

        // Assert
        Assert.AreNotSame(firstTransientService, secondTransientService);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ServiceLocator_GetNonExistentService_ShouldReturnNull()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var nonExistentService = locator.GetService<INonExistentService>();
    }

    [TestMethod]
    public void ServiceLocator_Dispose_ShouldCleanupCorrectly()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var locator = ServiceLocator.Default;

        // Act
        var scopedService = locator.GetService<ITestScopedService>();
        locator.Dispose();

        // Assert - Verify that getting a new instance works after dispose
        locator.CreateNewScope();
        var newScopedService = locator.GetService<ITestScopedService>();
        Assert.IsNotNull(newScopedService);
    }
}