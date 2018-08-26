using System;
using System.Collections.Generic;

namespace ISynergy.Services
{
    public interface ITelemetryService
    {
        object Client { get; }

        string Id { get; set; }
        string Account_Id { get; set; }

        void TrackException(Exception e);

        void TrackEvent(string e);

        void TrackEvent(string e, Dictionary<string, string> prop);

        void TrackPageView(string e);

        void Flush();
    }
}