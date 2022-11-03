using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Telemetry.ApplicationInsights.Options;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Telemetry.ApplicationInsights.Services
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
        private readonly ApplicationInsightsOptions _applicationInsightsOptions;
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="infoService">The information service.</param>
        /// <param name="options"></param>
        public TelemetryService(IContext context, IInfoService infoService, IOptions<ApplicationInsightsOptions> options)
        {
            _context = context;
            _applicationInsightsOptions = options.Value;

            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = _applicationInsightsOptions.ConnectionString;

            _client = new TelemetryClient(config);
            _client.Context.User.UserAgent = infoService.ProductName;
            _client.Context.Component.Version = infoService.ProductVersion.ToString();
            _client.Context.Session.Id = Guid.NewGuid().ToString();
            _client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => _client.Flush();
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
