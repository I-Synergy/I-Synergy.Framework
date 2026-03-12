namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

/// <summary>
/// Verifies that <see cref="CqrsSourceGenerator"/> emits correct query dispatch table
/// registration code for query handlers.
/// </summary>
[TestClass]
public class QueryDispatchTableGeneratorTests
{
    private const string QueryHandlerSource = """
        using ISynergy.Framework.CQRS.Queries;

        namespace MyApp.Handlers;

        public class FindUserQuery : IQuery<string> { }

        public class FindUserQueryHandler : IQueryHandler<FindUserQuery, string>
        {
            public System.Threading.Tasks.Task<string> HandleAsync(
                FindUserQuery query,
                System.Threading.CancellationToken cancellationToken = default)
                => System.Threading.Tasks.Task.FromResult("user");
        }
        """;

    /// <summary>
    /// A query handler should produce a dispatch table file.
    /// </summary>
    [TestMethod]
    public void Generator_EmitsDispatchTableFile_ForQueryHandlers()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);

        Assert.IsTrue(output.ContainsKey("CqrsQueryDispatchTable.g.cs"));
    }

    /// <summary>
    /// The dispatch table file must contain a <c>table.Register&lt;TQuery, TResult&gt;()</c> call
    /// for the discovered query handler.
    /// </summary>
    [TestMethod]
    public void Generator_DispatchTable_ContainsRegisterCall()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
        var source = output["CqrsQueryDispatchTable.g.cs"];

        StringAssert.Contains(source, "table.Register<");
        StringAssert.Contains(source, "FindUserQuery");
        StringAssert.Contains(source, "string");
    }

    /// <summary>
    /// The generated method must register the table as a singleton via <c>TryAddSingleton</c>.
    /// </summary>
    [TestMethod]
    public void Generator_DispatchTable_RegistersTryAddSingleton()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
        var source = output["CqrsQueryDispatchTable.g.cs"];

        StringAssert.Contains(source, "TryAddSingleton");
        StringAssert.Contains(source, "QueryDispatchTable");
    }

    /// <summary>
    /// When multiple query handlers exist, all must be registered in the dispatch table.
    /// </summary>
    [TestMethod]
    public void Generator_WithMultipleQueryHandlers_RegistersAll()
    {
        const string multiQuerySource = """
            using ISynergy.Framework.CQRS.Queries;
            using System.Threading;
            using System.Threading.Tasks;

            namespace MyApp.Handlers;

            public class QueryA : IQuery<int> { }
            public class QueryB : IQuery<string> { }

            public class HandlerA : IQueryHandler<QueryA, int>
            {
                public Task<int> HandleAsync(QueryA q, CancellationToken ct = default) => Task.FromResult(1);
            }

            public class HandlerB : IQueryHandler<QueryB, string>
            {
                public Task<string> HandleAsync(QueryB q, CancellationToken ct = default) => Task.FromResult("b");
            }
            """;

        var output = SourceGeneratorTestHelper.RunGenerator(multiQuerySource);
        var source = output["CqrsQueryDispatchTable.g.cs"];

        StringAssert.Contains(source, "QueryA");
        StringAssert.Contains(source, "QueryB");
    }

    /// <summary>
    /// When only command handlers (no query handlers) are present, no dispatch table file should be generated.
    /// </summary>
    [TestMethod]
    public void Generator_CommandOnlyHandlers_DoNotProduceDispatchTableFile()
    {
        const string commandOnlySource = """
            using ISynergy.Framework.CQRS.Commands;
            using System.Threading;
            using System.Threading.Tasks;

            namespace MyApp;
            public class Cmd : ICommand { }
            public class CmdHandler : ICommandHandler<Cmd>
            {
                public Task HandleAsync(Cmd c, CancellationToken ct = default) => Task.CompletedTask;
            }
            """;

        var output = SourceGeneratorTestHelper.RunGenerator(commandOnlySource);

        Assert.IsFalse(output.ContainsKey("CqrsQueryDispatchTable.g.cs"),
            "Dispatch table should only be generated when query handlers are present.");
    }
}
