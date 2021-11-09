using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Services.Options;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    /// <summary>
    /// Telemetry with AppCenter
    /// </summary>
    public class TelemetryService : ITelemetryService
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
        private readonly TelemetryOptions _appCenterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="options">The options.</param>
        public TelemetryService(IContext context, IInfoService infoService, IOptions<TelemetryOptions> options)
        {
            _context = context;
            _infoService = infoService;
            _appCenterOptions = options.Value;

            AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;
            AppCenter.Start(_appCenterOptions.Key, typeof(Analytics), typeof(Crashes));
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
                metrics.Add(nameof(_infoService.ProductName), _infoService.ProductName);
                metrics.Add(nameof(_infoService.ProductVersion), _infoService.ProductVersion.ToString());
            }

            return metrics;
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public Task TrackEventAsync(string e)
        {
            Analytics.TrackEvent(e, GetMetrics());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="props">The props.</param>
        /// <returns>Task.</returns>
        public Task TrackEventAsync(string e, Dictionary<string, string> props)
        {
            var metrics = GetMetrics();
            props.ForEach(p => metrics.Add(p.Key, p.Value));

            Analytics.TrackEvent(e, metrics);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the exception asynchronous.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task TrackExceptionAsync(Exception ex, string message)
        {
            if (ex is not null)
            {
                var metrics = GetMetrics();
                metrics.Add("Message", message);

                ///Send AppCenter exception.
                Crashes.TrackError(ex, metrics);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public Task TrackPageViewAsync(string e)
        {
            Analytics.TrackEvent(e, GetMetrics());
            return Task.CompletedTask;
        }
    }
}
