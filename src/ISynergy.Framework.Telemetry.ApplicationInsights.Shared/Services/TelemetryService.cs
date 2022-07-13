using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Telemetry.Services
{
    /// <summary>
    /// See https://www.meziantou.net/use-application-insights-in-a-desktop-application.htm
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
        private readonly TelemetryClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="infoService">The information service.</param>
        /// <param name="telemetryClient"></param>
        public TelemetryService(IContext context, IInfoService infoService, TelemetryClient telemetryClient)
        {
            _context = context;
            _client = telemetryClient;
            _client.Context.User.UserAgent = infoService.ProductName;
            _client.Context.Component.Version = infoService.ProductVersion.ToString();
            _client.Context.Session.Id = Guid.NewGuid().ToString();
            _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
        }

        /// <summary>
        /// Sets profile in telemetry context.
        /// </summary>
        private void GetUserProfile()
        {
            if (_context.IsAuthenticated && _context.CurrentProfile is IProfile profile)
            {
                _client.Context.User.Id = profile.Username;
                _client.Context.User.AccountId = profile.AccountDescription;
            }
            else
            {
                _client.Context.User.Id = string.Empty;
                _client.Context.User.AccountId = string.Empty;
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            GetUserProfile();
            _client?.Flush();
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public void TrackEvent(string e)
        {
            GetUserProfile();
            _client.TrackEvent(e);
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
            _client.TrackEvent(e, props, null);
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
                _client.TrackException(new ExceptionTelemetry { Exception = ex, Message = message });
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
            _client.TrackPageView(e);
        }
    }
}
