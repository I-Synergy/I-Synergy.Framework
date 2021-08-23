using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Framework.Automations.Options
{
    public class AutomationOptions
    {
        public TimeSpan DefaultTimeout { get; set; }
        public TimeSpan DefaultRefreshRate { get; set; }
        public TimeSpan DefaultQueueRefreshRate { get; set; }
    }
}
