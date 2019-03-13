using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Entities.Accounts
{
    public interface IApiKey
    {
        Guid ApiKey_Id { get; set; }
        Guid Tenant_Id { get; set; }
        string Key { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        string CreatedBy { get; set; }
    }
}
