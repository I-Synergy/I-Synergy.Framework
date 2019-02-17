using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy
{
    public class Context : ContextBase, IContext
    {
        public string ApiUrl { get; set; }
        public string AccountUrl { get; set; }
        public string WebUrl { get; set; }
        public string MonitorUrl { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
        public string TokenUrl { get; set; }
    }
}
