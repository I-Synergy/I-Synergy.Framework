using ISynergy.Models.Base;

namespace ISynergy.Entities.Base
{
    public interface IBaseEntity : IBaseClass
    {
        string Memo { get; set; }
        string InputFirst { get; set; }
        string InputLast { get; set; }
    }
}
