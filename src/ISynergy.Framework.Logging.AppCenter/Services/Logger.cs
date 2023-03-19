using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Logging.AppCenter.Options;
using ISynergy.Framework.Logging.Base;
using ISynergy.Framework.Logging.Extensions;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.Logging.AppCenter.Services
{
    public class Logger : BaseLogger
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
        /// Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="options">The options.</param>
        public Logger(IContext context, IInfoService infoService, IOptions<AppCenterOptions> options)
            : base("AppCenter Logger")
        {
            _context = context;
            _infoService = infoService;
            _appCenterOptions = options.Value;

            Microsoft.AppCenter.AppCenter.LogLevel = Microsoft.AppCenter.LogLevel.Verbose;
            Microsoft.AppCenter.AppCenter.Start(_appCenterOptions.Key, typeof(Analytics), typeof(Crashes));
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
                Microsoft.AppCenter.AppCenter.SetUserId(profile.Username);
                Microsoft.AppCenter.AppCenter.SetCountryCode(profile.CountryCode);

                metrics.Add(nameof(profile.Username), profile.Username);
                metrics.Add(nameof(profile.UserId), profile.UserId.ToString());
                metrics.Add(nameof(profile.AccountId), profile.AccountId.ToString());
                metrics.Add(nameof(profile.AccountDescription), profile.AccountDescription);
            }

            metrics.Add(nameof(_infoService.ProductName), _infoService.ProductName);
            metrics.Add(nameof(_infoService.ProductVersion), _infoService.ProductVersion.ToString());

            return metrics;
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            base.Log(logLevel, eventId, state, exception, formatter);

            var message = formatter(state, exception);

            var metrics = GetMetrics();
            metrics.Add("Message", message);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                if (logLevel == LogLevel.Error)
                {
                    Crashes.TrackError(exception, metrics);
                }
                else
                {
                    Analytics.TrackEvent(logLevel.ToLogLevelString(), metrics);
                }
            }
        }
    }
}
