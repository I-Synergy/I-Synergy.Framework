using ISynergy.Models.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Services
{
    public interface IFactoryService
    {
        string Name { get; set; }
        string Keywords { get; set; }
        string Title { get; set; }
        string PhoneNumber { get; set; }
        string Address { get; set; }
        string EmailAddress { get; set; }
        
        Token Token { get; set; }
        string CurrencyCode { get; set; }
        string CurrencySymbol { get; set; }
    }
}
