using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.CQRS.Extensions.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddCQRS_RegistersDispatchers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCQRS();
        var provider = services.BuildServiceProvider();

        // Assert
        var commandDispatcher = provider.GetService<ICommandDispatcher>();
        var queryDispatcher = provider.GetService<IQueryDispatcher>();

        Assert.IsNotNull(commandDispatcher);
        Assert.IsNotNull(queryDispatcher);
    }

    [TestMethod]
    public void AddHandlers_RegistersAllHandlersInAssembly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCQRS();

        // Act
        services.AddHandlers(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();

        // Assert
        var commandHandler = provider.GetService<ICommandHandler<TestCommand>>();
        var commandWithResultHandler = provider.GetService<ICommandHandler<TestCommandWithResult, string>>();
        var queryHandler = provider.GetService<IQueryHandler<TestQuery, string>>();

        Assert.IsNotNull(commandHandler);
        Assert.IsNotNull(commandWithResultHandler);
        Assert.IsNotNull(queryHandler);
    }

    [TestMethod]
    public async Task CompleteIntegrationTest_CommandDispatcherWithHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCQRS();
        services.AddHandlers(typeof(TestCommandHandler).Assembly);
        var provider = services.BuildServiceProvider();

        var commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();
        var command = new TestCommandWithResult { Input = "Integration Test" };

        // Act
        var result = await commandDispatcher.DispatchAsync<TestCommandWithResult, string>(command);

        // Assert
        Assert.AreEqual("Result: Integration Test", result);
    }

    [TestMethod]
    public async Task CompleteIntegrationTest_QueryDispatcherWithHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCQRS();
        services.AddHandlers(typeof(TestQueryHandler).Assembly);
        var provider = services.BuildServiceProvider();

        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
        var query = new TestQuery { Parameter = "Integration Query" };

        // Act
        var result = await queryDispatcher.DispatchAsync<string>(query);

        // Assert
        Assert.AreEqual("Query Result: Integration Query", result);
    }
}