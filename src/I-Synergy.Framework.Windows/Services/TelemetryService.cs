using ISynergy.Services;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;

namespace ISynergy
{
    public class TelemetryService : ITelemetryService
    {
        public object Client { get; }

        public string Id
        {
            get
            {
                return ((TelemetryClient)Client)?.Context.User.Id;
            }

            set
            {
                if ((TelemetryClient)Client != null)
                {
                    ((TelemetryClient)Client).Context.User.Id = value;
                }
            }
        }

        public string Account_Id
        {
            get
            {
                return ((TelemetryClient)Client)?.Context.User.AccountId;
            }

            set
            {
                if ((TelemetryClient)Client != null)
                {
                    ((TelemetryClient)Client).Context.User.AccountId = value;
                }
            }
        }

        public TelemetryService()
        {
            Client = new TelemetryClient();
        }

        public void Flush()
        {
            ((TelemetryClient)Client)?.Flush();
        }

        public void TrackEvent(string e)
        {
            ((TelemetryClient)Client)?.TrackEvent(e);
        }

        public void TrackEvent(string e, Dictionary<string, string> prop)
        {
            ((TelemetryClient)Client)?.TrackEvent(e, prop, null);
        }

        public void TrackException(Exception e)
        {
            ((TelemetryClient)Client)?.TrackException(e);
        }

        public void TrackPageView(string e)
        {
            ((TelemetryClient)Client)?.TrackPageView(e);
        }
    }
}