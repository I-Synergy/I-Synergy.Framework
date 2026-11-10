using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Decorators;
using ISynergy.Framework.CQRS.Messages;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

#pragma warning disable IL2026, IL3050 // the decorator registrations are documented as not trim/AOT compatible

namespace ISynergy.Framework.CQRS.Extensions;

/// <summary>
/// Tests for the decorator registration helpers in <see cref="ServiceCollectionExtensions"/>.
/// <see cref="ServiceCollectionExtensions.AddCommandNotifications"/> and
/// <see cref="ServiceCollectionExtensions.AddCQRSLogging"/> were refactored to project the collected
/// service descriptors onto their service types; these tests pin the resulting registrations.
/// </summary>
[TestClass]
public class ServiceCollectionDecorationTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public void AddCQRS_RegistersScopedDispatchers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCQRS();

        // Assert
        Assert.AreEqual(
            ServiceLifetime.Scoped,
            services.Single(descriptor => descriptor.ServiceType == typeof(ICommandDispatcher)).Lifetime);
        Assert.AreEqual(
            ServiceLifetime.Scoped,
            services.Single(descriptor => descriptor.ServiceType == typeof(IQueryDispatcher)).Lifetime);
    }

    [TestMethod]
    public void AddHandlers_RegistersHandlersAsScoped()
    {
        // Arrange
        var services = CreateServices();

        // Act & Assert
        Assert.AreEqual(
            ServiceLifetime.Scoped,
            services.Single(descriptor => descriptor.ServiceType == typeof(ICommandHandler<TestCommand>)).Lifetime);
        Assert.AreEqual(
            ServiceLifetime.Scoped,
            services.Single(descriptor => descriptor.ServiceType == typeof(IQueryHandler<TestQuery, string>)).Lifetime);
    }

    [TestMethod]
    public async Task AddCQRSLogging_DecoratesCommandHandlersOnly()
    {
        // Arrange
        var services = CreateServices();

        // Act
        services.AddCQRSLogging();
        using var provider = services.BuildServiceProvider();

        // Assert - command handlers are wrapped, query handlers are not
        Assert.IsInstanceOfType<LoggingCommandHandlerDecorator<TestCommand>>(
            provider.GetRequiredService<ICommandHandler<TestCommand>>());
        Assert.IsInstanceOfType<TestQueryHandler>(
            provider.GetRequiredService<IQueryHandler<TestQuery, string>>());

        // The decorator still delegates to the wrapped handler
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();
        await dispatcher.DispatchAsync(new TestCommand(), TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task AddCommandNotifications_DecoratesCommandHandlersWithAndWithoutResult()
    {
        // Arrange
        var services = CreateServices(out var messenger);

        // Act
        services.AddCommandNotifications();
        using var provider = services.BuildServiceProvider();

        // Assert - both handler shapes are decorated
        Assert.IsInstanceOfType<NotificationCommandHandlerDecorator<TestCommand>>(
            provider.GetRequiredService<ICommandHandler<TestCommand>>());
        Assert.IsInstanceOfType<NotificationCommandHandlerDecorator<TestCommandWithResult, string>>(
            provider.GetRequiredService<ICommandHandler<TestCommandWithResult, string>>());

        // Decorators forward to the inner handler and publish the notifications
        var dispatcher = provider.GetRequiredService<ICommandDispatcher>();
        await dispatcher.DispatchAsync(new TestCommand(), TestContext.CancellationToken);
        var result = await dispatcher.DispatchAsync<TestCommandWithResult, string>(
            new TestCommandWithResult { Input = "notified" }, TestContext.CancellationToken);

        Assert.AreEqual("Result: notified", result);
        messenger.Verify(
            x => x.Send(It.Is<CommandMessage<string>>(message =>
                message.Content.Contains(nameof(TestCommand), StringComparison.Ordinal))),
            Times.Once);
        messenger.Verify(
            x => x.Send(It.Is<CommandMessage<string>>(message => message.Content == result)),
            Times.Once);
    }

    [TestMethod]
    public void AddCommandNotifications_WithoutRegisteredHandlers_IsNoOp()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCommandNotifications();
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.IsNull(provider.GetService<ICommandHandler<TestCommand>>());
    }

    [TestMethod]
    public void AddCQRS_IsIdempotent()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - TryAddScoped must not add duplicate descriptors
        services.AddCQRS();
        services.AddCQRS();

        // Assert
        Assert.ContainsSingle(services.Where(descriptor => descriptor.ServiceType == typeof(ICommandDispatcher)));
        Assert.ContainsSingle(services.Where(descriptor => descriptor.ServiceType == typeof(IQueryDispatcher)));
    }

    #region Helpers

    private static ServiceCollection CreateServices() => CreateServices(out _);

    private static ServiceCollection CreateServices(out Mock<IMessengerService> messenger)
    {
        messenger = new Mock<IMessengerService>();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(messenger.Object);
        services.AddCQRS();
        services.AddHandlers(typeof(TestCommandHandler).Assembly);

        return services;
    }

    #endregion
}

#pragma warning restore IL2026, IL3050
