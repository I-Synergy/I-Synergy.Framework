using System;

namespace ISynergy.Framework.Core.Models.Tests
{
    public class ObservedFile
    {
        public string TimeStamp { get; private set; }
        public string EventName { get; private set; }
        public string FilterName { get; private set; }
        public string FileName { get; private set; }

        public ObservedFile(string eventName, string filterName, string fileName)
        {
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
            EventName = eventName;
            FilterName = filterName;
            FileName = fileName;
        }
    }
}
