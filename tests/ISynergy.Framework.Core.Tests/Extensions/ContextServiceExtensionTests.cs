using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ContextServiceExtensionTests
{
    private IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<IOrderProcessor, OrderProcessor>();
        services.AddScoped<IOrderValidator, OrderValidator>();

        return services.BuildServiceProvider();
    }

    [TestMethod]
    public void ExecuteInContext_ShouldExecuteSynchronously()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var wasExecuted = false;

        // Act
        serviceProvider.ExecuteInContext(
            context =>
            {
                var service = context.GetService<ITestService>();
                service.DoSomething();
                wasExecuted = true;
            });

        // Assert
        Assert.IsTrue(wasExecuted);
    }

    [TestMethod]
    public async Task ExecuteInContextAsync_ShouldExecuteAsynchronously()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        var wasExecuted = false;

        // Act
        await serviceProvider.ExecuteInContextAsync(
            async context =>
            {
                var service = context.GetService<ITestService>();
                await service.DoSomethingAsync();
                wasExecuted = true;
            });

        // Assert
        Assert.IsTrue(wasExecuted);
    }

    [TestMethod]
    public void ExecuteInContext_ShouldCreateNewScopeForEachExecution()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();
        ITestService? firstInstance = null;
        ITestService? secondInstance = null;

        // Act
        serviceProvider.ExecuteInContext(
            context =>
            {
                firstInstance = context.GetService<ITestService>();
            });

        serviceProvider.ExecuteInContext(
            context =>
            {
                secondInstance = context.GetService<ITestService>();
            });

        // Assert
        Assert.IsNotNull(firstInstance);
        Assert.IsNotNull(secondInstance);
        Assert.AreNotSame(firstInstance, secondInstance);
    }

    [TestMethod]
    public void ExecuteInContext_WithMultipleServices_ShouldResolveAll()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();

        // Act & Assert
        serviceProvider.ExecuteInContext(
            context =>
            {
                var processor = context.GetService<IOrderProcessor>();
                var validator = context.GetService<IOrderValidator>();

                Assert.IsNotNull(processor);
                Assert.IsNotNull(validator);
            });
    }

    [TestMethod]
    public void ExecuteInContext_ShouldDisposeScope()
    {
        // Arrange
        var services = new ServiceCollection();
        var disposableService = new DisposableTestService();

        services.AddScoped(_ => disposableService);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        serviceProvider.ExecuteInContext(
            context =>
            {
                var service = context.GetService<DisposableTestService>();
                Assert.IsNotNull(service);
            });

        // Assert
        Assert.IsTrue(disposableService.WasDisposed);
    }

    [TestMethod]
    public void ExecuteInContext_WithUnregisteredService_ShouldThrow()
    {
        // Arrange
        var serviceProvider = CreateTestServiceProvider();

        // Act
        Assert.Throws<InvalidOperationException>(() =>
        serviceProvider.ExecuteInContext(
            context =>
            {
                // Try to resolve unregistered service
                var unregistered = context.GetService<IUnregisteredService>();
            }));
    }

    #region Test interfaces and implementations
    public interface ITestService
    {
        void DoSomething();
        Task DoSomethingAsync();
    }

    public class TestService : ITestService
    {
        public void DoSomething() { }
        public Task DoSomethingAsync() => Task.CompletedTask;
    }

    public class DisposableTestService : IDisposable
    {
        public bool WasDisposed { get; private set; } = false;
        public void Dispose() => WasDisposed = true;
    }

    public interface IUnregisteredService { }

    public interface IOrderProcessor { }
    public class OrderProcessor : IOrderProcessor { }

    public interface IOrderValidator { }
    public class OrderValidator : IOrderValidator { }

    public class Order
    {
        public int Id { get; set; }
    }
    #endregion
}
