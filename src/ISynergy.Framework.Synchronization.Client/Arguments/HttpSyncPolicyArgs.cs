using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client
{
    public class HttpSyncPolicyArgs : ProgressArgs
    {
        public HttpSyncPolicyArgs(SyncContext context, int retryCount, int retryNumber, TimeSpan delay)
            : base(context, null, null)
        {
            RetryCount = retryCount;
            RetryNumber = retryNumber;
            Delay = delay;
        }

        public override string Message => $"Retry Sending Http Request ({RetryNumber}/{RetryCount}. Waiting {Delay.TotalMilliseconds}ms... )";
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Trace;
        public override int EventId => HttpClientSyncEventsId.HttpSyncPolicy.Id;

        /// <summary>
        /// Gets the max number of retry for this http request
        /// </summary>
        public int RetryCount { get; }

        /// <summary>
        /// Gets the current retry number
        /// </summary>
        public int RetryNumber { get; }

        /// <summary>
        /// Gets the delay until next retry
        /// </summary>
        public TimeSpan Delay { get; }
    }

    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpSyncPolicy => new EventId(20500, nameof(HttpSyncPolicy));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when client is trying to send again an http request message 
        /// </summary>
        public static void OnHttpPolicyRetrying(this WebClientOrchestrator orchestrator,
            Action<HttpSyncPolicyArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when client is trying to send again an http request message 
        /// </summary>
        public static void OnHttpPolicyRetrying(this WebClientOrchestrator orchestrator,
            Func<HttpSyncPolicyArgs, Task> action)
            => orchestrator.SetInterceptor(action);
    }
    }
