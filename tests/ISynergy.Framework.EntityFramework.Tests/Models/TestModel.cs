using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.EntityFramework.Tests.Models;

internal class TestModel : BaseModel
{
    [Identity]
    public int Id { get; set; }
}
