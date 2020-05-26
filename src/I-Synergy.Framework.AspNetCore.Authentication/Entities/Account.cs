using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    public class Account : ClassBase
    {
        public Account()
        {
            ApiKeys = new HashSet<ApiKey>();
            AccountModules = new HashSet<AccountModule>();
            Users = new HashSet<User>();
        }

        [Required] [Identity] public Guid AccountId { get; set; }
        [Required] public Guid RelationId { get; set; }
        [Required] [StringLength(128)] public string Description { get; set; }
        [Required] public int UsersAllowed { get; set; }
        [Required] public DateTimeOffset RegistrationDate { get; set; }
        [Required] public DateTimeOffset ExpirationDate { get; set; }
        [Required] public string TimeZoneId { get; set; }
        [Required] public bool IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ChangedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? ChangedBy { get; set; }

        public ICollection<ApiKey> ApiKeys { get; set; }
        public ICollection<AccountModule> AccountModules { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
