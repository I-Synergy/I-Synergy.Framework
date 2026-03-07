using ISynergy.Framework.CQRS.Commands;

namespace ISynergy.Framework.CQRS.TestImplementations;

public class TestCommand : ICommand
{
    public string Data { get; set; } = "Test Command";
}

public class TestCommandWithResult : ICommand<string>
{
    public string Input { get; set; } = "Test Input";
}