using ISynergy.Framework.CQRS.Queries;

namespace ISynergy.Framework.CQRS.TestImplementations;

public class TestQuery : IQuery<string>
{
    public string Parameter { get; set; } = "Test Query";
}