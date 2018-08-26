using Microsoft.Extensions.Logging;
using PubnubApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Services
{
    public class PlatformPubNubLog : IPubnubLog
    {
        private readonly ILogger Logger;

        public PlatformPubNubLog(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<PlatformPubNubLog>();
        }

        public void WriteToLog(string logText)
        {
            Logger.LogTrace(logText);
        }
    }
}
