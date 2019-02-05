using ISynergy.Models.Base;

namespace ISynergy.Entities.Base
{
    public interface IEntityBase : IClassBase
    {
        string Memo { get; set; }
        string InputFirst { get; set; }
        string InputLast { get; set; }
    }
}
