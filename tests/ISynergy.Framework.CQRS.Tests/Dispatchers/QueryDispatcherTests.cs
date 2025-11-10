using ISynergy.Framework.CQRS.Queries;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ISynergy.Framework.CQRS.Dispatchers.Tests;

[TestClass]
public class QueryDispatcherTests
{
    [TestMethod]
    public async Task QueryDispatcher_DispatchesQuery_ReturnsExpectedResult()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<TestQuery, string>>(_ =>
            new TestQueryHandler());
        var provider = services.BuildServiceProvider();

        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());
        var query = new TestQuery { Parameter = "Test Parameter" };

        // Act
        var result = await dispatcher.DispatchAsync<string>(query);

        // Assert
        Assert.AreEqual("Query Result: Test Parameter", result);
    }

    [TestMethod]
    public async Task QueryDispatcher_MissingHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());
        var query = new TestQuery();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await dispatcher.DispatchAsync<string>(query));
    }

    [TestMethod]
    public async Task QueryDispatcher_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await dispatcher.DispatchAsync<string>((IQuery<string>?)null!));
    }

    public class AnotherQuery : IQuery<int> { public int Value { get; set; } }
    public class AnotherQueryHandler : IQueryHandler<AnotherQuery, int>
    {
        public Task<int> HandleAsync(AnotherQuery query, CancellationToken cancellationToken = default)
            => Task.FromResult(query.Value * 2);
    }

    [TestMethod]
    public async Task QueryDispatcher_CanHandleMultipleQueryTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<TestQuery, string>>(_ => new TestQueryHandler());
        services.AddScoped<IQueryHandler<AnotherQuery, int>>(_ => new AnotherQueryHandler());
        var provider = services.BuildServiceProvider();
        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());

        // Act
        var stringResult = await dispatcher.DispatchAsync<string>(new TestQuery { Parameter = "Multi" });
        var intResult = await dispatcher.DispatchAsync<int>(new AnotherQuery { Value = 21 });

        // Assert
        Assert.AreEqual("Query Result: Multi", stringResult);
        Assert.AreEqual(42, intResult);
    }

    public class CancellableQuery : IQuery<string> { }
    public class CancellableQueryHandler : IQueryHandler<CancellableQuery, string>
    {
        public Task<string> HandleAsync(CancellableQuery query, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult("cancelled");
        }
    }

    [TestMethod]
    public async Task QueryDispatcher_PassesCancellationTokenToHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IQueryHandler<CancellableQuery, string>>(_ => new CancellableQueryHandler());
        var provider = services.BuildServiceProvider();
        var dispatcher = new QueryDispatcher(provider, Mock.Of<ILogger<QueryDispatcher>>());
        var query = new CancellableQuery();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await dispatcher.DispatchAsync<string>(query, cts.Token));
    }
}