using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Entities
{
    public class AccountModule
    {
        public Guid AccountId { get; set; }
        public Guid ModuleId { get; set; }

        public Account Account { get; set; }
        public Module Module { get; set; }
    }
}
