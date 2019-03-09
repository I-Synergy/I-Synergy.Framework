using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Background.Tasks.Events
{
    public class JobExceptionInfo
    {
        public string Name { get; set; }
        public Exception Exception { get; set; }
    }
}
