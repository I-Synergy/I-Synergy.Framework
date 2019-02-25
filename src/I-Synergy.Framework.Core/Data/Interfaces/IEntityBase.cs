using System;

namespace ISynergy
{
    public interface IEntityBase : IClassBase
    {
        string Memo { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset? ChangedDate { get; set; }
        string CreatedBy { get; set; }
        string ChangedBy { get; set; }
    }
}
