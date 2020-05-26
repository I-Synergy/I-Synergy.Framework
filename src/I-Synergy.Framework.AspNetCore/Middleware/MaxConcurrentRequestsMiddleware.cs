using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.Enumerations;
using ISynergy.Framework.AspNetCore.Middleware.Internals;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Middleware
{
    /// <summary>
    /// Class MaxConcurrentRequestsMiddleware.
    /// </summary>
    public class MaxConcurrentRequestsMiddleware
    {
        /// <summary>
        /// The concurrent requests count
        /// </summary>
        private int _concurrentRequestsCount;

        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// The options
        /// </summary>
        private readonly MaxConcurrentRequestsOptions _options;
        /// <summary>
        /// The enqueuer
        /// </summary>
        private readonly MaxConcurrentRequestsEnqueuer _enqueuer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxConcurrentRequestsMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="options">The options.</param>
        public MaxConcurrentRequestsMiddleware(RequestDelegate next, IOptions<MaxConcurrentRequestsOptions> options)
        {
            Argument.IsNotNull(nameof(MaxConcurrentRequestsOptions), options.Value);
            Argument.IsNotNull(nameof(next), next);

            _concurrentRequestsCount = 0;
            _next = next;
            _options = options.Value;

            if (_options.LimitExceededPolicy != MaxConcurrentRequestsLimitExceededPolicy.Drop)
            {
                _enqueuer = new MaxConcurrentRequestsEnqueuer(_options.MaxQueueLength, (MaxConcurrentRequestsEnqueuer.DropMode)_options.LimitExceededPolicy, _options.MaxTimeInQueue);
            }
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task Invoke(HttpContext context)
        {
            if (CheckLimitExceeded() && !(await TryWaitInQueueAsync(context.RequestAborted).ConfigureAwait(false)))
            {
                if (!context.RequestAborted.IsCancellationRequested)
                {
                    var responseFeature = context.Features.Get<IHttpResponseFeature>();
                    responseFeature.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    responseFeature.ReasonPhrase = "Concurrent request limit exceeded.";
                }
            }
            else
            {
                try
                {
                    await _next(context).ConfigureAwait(false);
                }
                finally
                {
                    if (await ShouldDecrementConcurrentRequestsCountAsync().ConfigureAwait(false))
                    {
                        Interlocked.Decrement(ref _concurrentRequestsCount);
                    }
                }
            }
        }

        /// <summary>
        /// Checks the limit exceeded.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckLimitExceeded()
        {
            bool limitExceeded;

            if (_options.Limit == MaxConcurrentRequestsOptions.ConcurrentRequestsUnlimited)
            {
                limitExceeded = false;
            }
            else
            {
                int initialConcurrentRequestsCount, incrementedConcurrentRequestsCount;
                do
                {
                    limitExceeded = true;

                    initialConcurrentRequestsCount = _concurrentRequestsCount;
                    if (initialConcurrentRequestsCount >= _options.Limit)
                    {
                        break;
                    }

                    limitExceeded = false;
                    incrementedConcurrentRequestsCount = initialConcurrentRequestsCount + 1;
                }
                while (initialConcurrentRequestsCount != Interlocked.CompareExchange(ref _concurrentRequestsCount, incrementedConcurrentRequestsCount, initialConcurrentRequestsCount));
            }

            return limitExceeded;
        }

        /// <summary>
        /// try wait in queue as an asynchronous operation.
        /// </summary>
        /// <param name="requestAbortedCancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> TryWaitInQueueAsync(CancellationToken requestAbortedCancellationToken)
        {
            return (_enqueuer != null) && (await _enqueuer.EnqueueAsync(requestAbortedCancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// should decrement concurrent requests count as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> ShouldDecrementConcurrentRequestsCountAsync()
        {
            return (_options.Limit != MaxConcurrentRequestsOptions.ConcurrentRequestsUnlimited)
                && ((_enqueuer == null) || !(await _enqueuer.DequeueAsync().ConfigureAwait(false)));
        }
    }
}
