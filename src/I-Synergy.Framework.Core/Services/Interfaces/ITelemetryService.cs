using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface ITelemetryService
    {
        string UserId { get; set; }
        string AccountId { get; set; }
        Task TrackExceptionAsync(Exception e);
        Task TrackEventAsync(string e);
        Task TrackEventAsync(string e, Dictionary<string, string> prop);
        Task TrackPageViewAsync(string e);
        void Flush();
    }
}