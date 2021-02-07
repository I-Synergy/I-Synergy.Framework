using ISynergy.Framework.Mvvm.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Services
{
    /// <summary>
    /// See https://www.meziantou.net/use-application-insights-in-a-desktop-application.htm
    /// </summary>
    public class TelemetryService : ITelemetryService
    {
        public void Flush()
        {
            throw new NotImplementedException();
        }

        public Task TrackEventAsync(string e)
        {
            throw new NotImplementedException();
        }

        public Task TrackEventAsync(string e, Dictionary<string, string> prop)
        {
            throw new NotImplementedException();
        }

        public Task TrackExceptionAsync(Exception e, string message)
        {
            throw new NotImplementedException();
        }

        public Task TrackPageViewAsync(string e)
        {
            throw new NotImplementedException();
        }
    }
}
