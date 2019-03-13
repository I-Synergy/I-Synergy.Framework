using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Entities.Accounts
{
    public interface IApiKey
    {
        Guid ApiKey_Id { get; set; }
        string User_Id { get; set; }
        string Key { get; set; }
        string Description { get; set; }
        DateTimeOffset ExpirationDate { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        string CreatedBy { get; set; }
    }
}
