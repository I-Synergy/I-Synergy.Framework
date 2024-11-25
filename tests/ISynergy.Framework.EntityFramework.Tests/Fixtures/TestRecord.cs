using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.EntityFramework.Tests.Fixtures;

internal record TestRecord : BaseRecord
{
    [Identity]
    public int Id { get; set; }
}
