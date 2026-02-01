using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Tests.Enumerations;

namespace ISynergy.Framework.Core.Tests.Data;

public class Space
{
    [Identity]
    public Guid Id { get; set; }

    [ParentIdentity(typeof(Guid))]
    public Guid ParentId { get; set; }

    public string Name { get; set; } = string.Empty;
    public double SquareFeet { get; set; }
    public SpaceTypes Type { get; set; }

    public Space()
    {
        Id = Guid.NewGuid();
    }
}
