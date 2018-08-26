using ISynergy.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISynergy.Services
{
    public class FactoryService : IFactoryService
    {
        public string Name { get; set; }
        public string Keywords { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }

        public Token Token { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
