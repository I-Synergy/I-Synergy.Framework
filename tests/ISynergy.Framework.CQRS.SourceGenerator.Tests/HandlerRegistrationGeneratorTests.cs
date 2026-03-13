namespace ISynergy.Framework.CQRS.SourceGenerator.Tests;

/// <summary>
/// Verifies that <see cref="CqrsSourceGenerator"/> emits correct handler registration
/// code for command and query handlers.
/// </summary>
[TestClass]
public class HandlerRegistrationGeneratorTests
{
    private const string CommandHandlerSource = """
        using ISynergy.Framework.CQRS.Commands;

        namespace MyApp.Handlers;

        public class DoThingCommand : ICommand { }

        public class DoThingCommandHandler : ICommandHandler<DoThingCommand>
        {
            public System.Threading.Tasks.Task HandleAsync(
                DoThingCommand command,
                System.Threading.CancellationToken cancellationToken = default)
                => System.Threading.Tasks.Task.CompletedTask;
        }
        """;

    private const string QueryHandlerSource = """
        using ISynergy.Framework.CQRS.Queries;

        namespace MyApp.Handlers;

        public class GetItemQuery : IQuery<string> { }

        public class GetItemQueryHandler : IQueryHandler<GetItemQuery, string>
        {
            public System.Threading.Tasks.Task<string> HandleAsync(
                GetItemQuery query,
                System.Threading.CancellationToken cancellationToken = default)
                => System.Threading.Tasks.Task.FromResult("item");
        }
        """;

    /// <summary>
    /// A command handler should cause <c>CqrsHandlerRegistrations.g.cs</c> to be emitted.
    /// </summary>
    [TestMethod]
    public void Generator_WithCommandHandler_EmitsRegistrationFile()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(CommandHandlerSource);

        Assert.IsTrue(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
            "Expected CqrsHandlerRegistrations.g.cs to be generated.");
    }

    /// <summary>
    /// The generated file must register the handler as a scoped service using its interface.
    /// </summary>
    [TestMethod]
    public void Generator_WithCommandHandler_RegistersHandlerAsScoped()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(CommandHandlerSource);
        var source = output["CqrsHandlerRegistrations.g.cs"];

        StringAssert.Contains(source, "AddScoped");
        StringAssert.Contains(source, "DoThingCommandHandler");
        StringAssert.Contains(source, "ICommandHandler");
        StringAssert.Contains(source, "DoThingCommand");
    }

    /// <summary>
    /// A query handler should cause both registration and dispatch-table files to be emitted.
    /// </summary>
    [TestMethod]
    public void Generator_WithQueryHandler_EmitsRegistrationAndDispatchTableFiles()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);

        Assert.IsTrue(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
            "Expected CqrsHandlerRegistrations.g.cs");
        Assert.IsTrue(output.ContainsKey("CqrsQueryDispatchTable.g.cs"),
            "Expected CqrsQueryDispatchTable.g.cs");
    }

    /// <summary>
    /// The query handler must be registered as scoped with the correct service and implementation types.
    /// </summary>
    [TestMethod]
    public void Generator_WithQueryHandler_RegistersQueryHandlerAsScoped()
    {
        var output = SourceGeneratorTestHelper.RunGenerator(QueryHandlerSource);
        var source = output["CqrsHandlerRegistrations.g.cs"];

        StringAssert.Contains(source, "AddScoped");
        StringAssert.Contains(source, "GetItemQueryHandler");
        StringAssert.Contains(source, "IQueryHandler");
    }

    /// <summary>
    /// When no handler types exist, the generator must emit no files.
    /// </summary>
    [TestMethod]
    public void Generator_WithNoHandlers_EmitsNoFiles()
    {
        const string noHandlerSource = """
            namespace MyApp;
            public class NotAHandler { }
            """;

        var output = SourceGeneratorTestHelper.RunGenerator(noHandlerSource);

        Assert.IsFalse(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
            "Should not generate when no handlers are present.");
    }

    /// <summary>
    /// Abstract handler classes must be excluded — they cannot be instantiated by DI.
    /// </summary>
    [TestMethod]
    public void Generator_WithAbstractHandler_IgnoresAbstractClass()
    {
        const string abstractSource = """
            using ISynergy.Framework.CQRS.Commands;

            namespace MyApp;
            public class MyCmd : ICommand { }
            public abstract class AbstractHandler : ICommandHandler<MyCmd>
            {
                public abstract System.Threading.Tasks.Task HandleAsync(
                    MyCmd command,
                    System.Threading.CancellationToken cancellationToken = default);
            }
            """;

        var output = SourceGeneratorTestHelper.RunGenerator(abstractSource);

        Assert.IsFalse(output.ContainsKey("CqrsHandlerRegistrations.g.cs"),
            "Abstract handlers should be ignored.");
    }
}
