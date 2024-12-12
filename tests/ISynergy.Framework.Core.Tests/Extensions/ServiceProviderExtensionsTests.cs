using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ServiceProviderExtensionsTests
{
    [TestMethod]
    public void GetRegisteredServices_ShouldReturnAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        services.AddSingleton<IAnotherService, AnotherService>();
        services.AddTransient<IThirdService, ThirdService>();

        var serviceProvider = services.BuildServiceProviderWithLocator();

        // Act
        var registeredServices = serviceProvider.GetRegisteredServices().ToList();

        // Assert
        Assert.IsTrue(registeredServices.Any());

        var testService = registeredServices
            .FirstOrDefault(x => x.ServiceType == typeof(ITestService));

        Assert.IsNotNull(testService);

        Assert.AreEqual(ServiceLifetime.Scoped, testService.Lifetime);

        var singletonService = registeredServices
            .FirstOrDefault(x => x.ServiceType == typeof(IAnotherService));

        Assert.IsNotNull(singletonService);

        Assert.AreEqual(ServiceLifetime.Singleton, singletonService.Lifetime);
    }

    [TestMethod]
    public void GetRegisteredServices_WithEmptyProvider_ShouldReturnEmptyCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProviderWithLocator();

        // Act
        var registeredServices = serviceProvider.GetRegisteredServices();

        // Assert
        Assert.IsFalse(registeredServices.Any());
    }
}

#region Test interfaces and classes
public interface ITestService { }
public class TestService : ITestService { }
public interface IAnotherService { }
public class AnotherService : IAnotherService { }
public interface IThirdService { }
public class ThirdService : IThirdService { }
#endregion