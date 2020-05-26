using System;
using System.ComponentModel.DataAnnotations;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Data;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    public class ApiKey : ClassBase
    {
        [Required] [Identity] public Guid ApiKeyId { get; set; }
        [Required] public Guid AccountId { get; set; }
        [Required] public string Key { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Account Account { get; set; }
    }
}
