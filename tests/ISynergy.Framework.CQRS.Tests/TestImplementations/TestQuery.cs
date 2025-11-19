using ISynergy.Framework.CQRS.Queries;

namespace ISynergy.Framework.CQRS.TestImplementations.Tests;

public class TestQuery : IQuery<string>
{
    public string Parameter { get; set; } = "Test Query";
}