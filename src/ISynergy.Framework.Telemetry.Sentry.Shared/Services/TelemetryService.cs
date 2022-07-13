using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using Sentry;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Telemetry.Services
{
    /// <summary>
    /// Telemetry Service for Sentry.io
    /// </summary>
    internal class TelemetryService : ITelemetryService
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IContext _context;
        /// <summary>
        /// Telemetry client for Application Insights.
        /// </summary>
        private readonly ISentryClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="infoService">The information service.</param>
        /// <param name="sentryClient"></param>
        public TelemetryService(IContext context, IInfoService infoService, ISentryClient sentryClient)
        {
            _context = context;
            _client = sentryClient;
        }

        /// <summary>
        /// Sets profile in telemetry context.
        /// </summary>
        private void GetUserProfile()
        {
            if (_context.IsAuthenticated && _context.CurrentProfile is IProfile profile)
            {
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.User = new User
                    {
                        Username = profile.Username,
                        Id = profile.UserId.ToString(),
                        Other = new Dictionary<string, string>()
                        {
                            { "AccountId", profile.AccountId.ToString() },
                            { "AccountDescription", profile.AccountDescription }
                        }
                    };
                });
            }
            else
            {
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.User = null;
                });
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            GetUserProfile();
            _client.FlushAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public void TrackEvent(string e)
        {
            GetUserProfile();
            var sentryEvent = new SentryEvent();
            sentryEvent.Message = e;
            _client.CaptureEvent(sentryEvent);
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="props">The props.</param>
        /// <returns>Task.</returns>
        public void TrackEvent(string e, Dictionary<string, string> props)
        {
            GetUserProfile();
            var sentryEvent = new SentryEvent();
            sentryEvent.Message = e;

            foreach (var item in props)
            {
                sentryEvent.SetExtra(item.Key, item.Value); 
            }
            
            _client.CaptureEvent(sentryEvent);
        }

        /// <summary>
        /// Tracks the exception asynchronous.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public void TrackException(Exception ex, string message)
        {
            if (ex is not null)
            {
                GetUserProfile();
                _client.CaptureException(new Exception(message, ex));
            }
        }

        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public void TrackPageView(string e)
        {
            GetUserProfile();
            _client.CaptureEvent(new SentryEvent { Message = e });
        }
    }
}
