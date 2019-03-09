using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks.Events
{
    public class JobEndInfo
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? NextRun { get; set; }
    }
}
