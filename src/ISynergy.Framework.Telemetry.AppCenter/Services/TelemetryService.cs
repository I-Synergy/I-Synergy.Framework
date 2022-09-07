using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.AppCenter.Options;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Telemetry.AppCenter.Services
{
    /// <summary>
    /// Telemetry for AppCenter
    /// </summary>
    internal class TelemetryService : ITelemetryService
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IContext _context;
        /// <summary>
        /// The information service
        /// </summary>
        private readonly IInfoService _infoService;

        /// <summary>
        /// The application center options
        /// </summary>
        private readonly AppCenterOptions _appCenterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="options">The options.</param>
        public TelemetryService(IContext context, IInfoService infoService, IOptions<AppCenterOptions> options)
        {
            _context = context;
            _infoService = infoService;
            _appCenterOptions = options.Value;

            Microsoft.AppCenter.AppCenter.LogLevel = LogLevel.Verbose;
            Microsoft.AppCenter.AppCenter.Start(_appCenterOptions.Key, typeof(Analytics), typeof(Crashes));
        }

        /// <summary>
        /// Sets profile in telemetry context.
        /// </summary>
        public void GetUserProfile()
        {
        }

        /// <summary>
        /// Gets the metrics.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        private Dictionary<string, string> GetMetrics()
        {
            var metrics = new Dictionary<string, string>();

            if (_context.IsAuthenticated && _context.CurrentProfile is IProfile profile)
            {
                metrics.Add(nameof(profile.Username), profile.Username);
                metrics.Add(nameof(profile.UserId), profile.UserId.ToString());
                metrics.Add(nameof(profile.AccountId), profile.AccountId.ToString());
                metrics.Add(nameof(profile.AccountDescription), profile.AccountDescription);
            }

            metrics.Add(nameof(_infoService.ProductName), _infoService.ProductName);
            metrics.Add(nameof(_infoService.ProductVersion), _infoService.ProductVersion.ToString());

            return metrics;
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush() { }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public void TrackEvent(string e)
        {
            Analytics.TrackEvent(e, GetMetrics());
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="props">The props.</param>
        /// <returns>Task.</returns>
        public void TrackEvent(string e, Dictionary<string, string> props)
        {
            var metrics = GetMetrics();
            props.ForEach(p => metrics.Add(p.Key, p.Value));

            Analytics.TrackEvent(e, metrics);
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
                var metrics = GetMetrics();
                metrics.Add("Message", message);

                //Send AppCenter exception.
                Crashes.TrackError(ex, metrics);
            }
        }

        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public void TrackPageView(string e)
        {
            Analytics.TrackEvent(e, GetMetrics());
        }
    }
}
