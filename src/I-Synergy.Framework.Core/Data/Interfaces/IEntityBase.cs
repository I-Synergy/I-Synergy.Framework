using System;

namespace ISynergy
{
    public interface IEntityBase : IClassBase
    {
        int Version { get; set; }
        string Memo { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset? ChangedDate { get; set; }
        string CreatedBy { get; set; }
        string ChangedBy { get; set; }

        [Obsolete] string InputFirst { get; set; }
        [Obsolete] string InputLast { get; set; }
    }
}
