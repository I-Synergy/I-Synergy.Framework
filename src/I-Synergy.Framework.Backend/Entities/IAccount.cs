using System;

namespace ISynergy.Entities.Accounts
{
    public interface IAccount : IClassBase
    {
        Guid Account_Id { get; set; }
        Guid Relation_Id { get; set; }
        string Description { get; set; }
        int UsersAllowed { get; set; }
        DateTimeOffset Registration_Date { get; set; }
        DateTimeOffset Expiration_Date { get; set; }
        string TimeZoneId { get; set; }
        bool IsActive { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset? ChangedDate { get; set; }
        string CreatedBy { get; set; }
        string ChangedBy { get; set; }
    }
}
