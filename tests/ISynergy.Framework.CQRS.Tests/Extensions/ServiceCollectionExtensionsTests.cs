using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations;
using ISynergy.Framework.CQRS.Tests; // generated CqrsHandlerRegistrations extension methods
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.CQRS.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddCQRS_RegistersDispatchers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

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
        services.AddLogging();
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
        services.AddLogging();
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
        services.AddLogging();
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

    [TestMethod]
    public async Task GeneratedCqrsHandlers_CommandDispatch_WorksEndToEnd()
    {
        // Arrange — use generated registration (no reflection)
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCQRS();
        services.AddCQRSHandlers();       // generated
        services.AddQueryDispatchTable(); // generated
        var provider = services.BuildServiceProvider();

        var commandDispatcher = provider.GetRequiredService<ICommandDispatcher>();
        var command = new TestCommandWithResult { Input = "AOT Integration Test" };

        // Act
        var result = await commandDispatcher.DispatchAsync<TestCommandWithResult, string>(command);

        // Assert
        Assert.AreEqual("Result: AOT Integration Test", result);
    }

    [TestMethod]
    public async Task GeneratedCqrsHandlers_QueryDispatch_UsesDispatchTable()
    {
        // Arrange — dispatch table must be registered for AOT-safe single-arg dispatch
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCQRS();
        services.AddCQRSHandlers();
        services.AddQueryDispatchTable();
        var provider = services.BuildServiceProvider();

        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
        var query = new TestQuery { Parameter = "AOT Query" };

        // Act — single-arg overload; QueryDispatchTable routes it without reflection
        var result = await queryDispatcher.DispatchAsync<string>(query);

        // Assert
        Assert.AreEqual("Query Result: AOT Query", result);
    }

    [TestMethod]
    public async Task GeneratedCqrsHandlers_QueryDispatch_DoublyGenericOverload_WorksWithoutTable()
    {
        // Arrange — doubly-generic overload is AOT-safe even without a dispatch table
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCQRS();
        services.AddCQRSHandlers(); // no AddQueryDispatchTable
        var provider = services.BuildServiceProvider();

        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher>();
        var query = new TestQuery { Parameter = "Doubly Generic" };

        // Act — TQuery is statically known, no MakeGenericType needed
        var result = await queryDispatcher.DispatchAsync<TestQuery, string>(query);

        // Assert
        Assert.AreEqual("Query Result: Doubly Generic", result);
    }
}