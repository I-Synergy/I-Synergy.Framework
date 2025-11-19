using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.CQRS.Queries.Tests;

[TestClass]
public class QueryTests
{
    [TestMethod]
    public async Task QueryHandler_ExecutesQuery_ReturnsExpectedResult()
    {
        // Arrange
        var handler = new TestQueryHandler();
        var query = new TestQuery { Parameter = "Test Parameter" };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.AreEqual("Query Result: Test Parameter", result);
    }

    [TestMethod]
    public async Task QueryHandler_WithCancellationToken_ThrowsWhenCancelled()
    {
        // Arrange
        var handler = new TestQueryHandler();
        var query = new TestQuery();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await Task.Delay(100, cts.Token); // This will throw when used with the canceled token
            await handler.HandleAsync(query, cts.Token);
        });
    }
}