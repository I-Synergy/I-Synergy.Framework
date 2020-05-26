using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface ITelemetryService
    {
        Task TrackExceptionAsync(Exception e, string message);
        Task TrackEventAsync(string e);
        Task TrackEventAsync(string e, Dictionary<string, string> prop);
        Task TrackPageViewAsync(string e);
        void Flush();
    }
}
