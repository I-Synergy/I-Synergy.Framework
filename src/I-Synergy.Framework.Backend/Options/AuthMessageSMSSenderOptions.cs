using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Options
{
    public class AuthMessageSMSSenderOptions
    {
        public string SID { get; set; }
        public string AuthToken { get; set; }
        public string SendNumber { get; set; }
    }
}
