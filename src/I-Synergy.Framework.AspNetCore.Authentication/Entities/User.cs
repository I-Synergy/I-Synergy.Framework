using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using Microsoft.AspNetCore.Identity;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    public class User : IdentityUser
    {
        [ParentIdentity(typeof(Guid))] [Required] public Guid AccountId { get; set; }
        public Guid RelationId { get; set; }
        public Account Account { get; set; }
    }
}
