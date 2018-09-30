using System;

namespace ISynergy.Entities.Accounts
{
    public interface IUser
    {
        DateTimeOffset? LockoutEnd { get; set; }
        bool TwoFactorEnabled { get; set; }
        bool PhoneNumberConfirmed { get; set; }
        string PhoneNumber { get; set; }
        string ConcurrencyStamp { get; set; }
        string SecurityStamp { get; set; }
        string PasswordHash { get; set; }
        bool EmailConfirmed { get; set; }
        string NormalizedEmail { get; set; }
        string Email { get; set; }
        string NormalizedUserName { get; set; }
        string UserName { get; set; }
        string Id { get; set; }
        bool LockoutEnabled { get; set; }
        int AccessFailedCount { get; set; }
        Guid Relation_Id { get; set; }
        Guid Account_Id { get; set; }
    }
}
