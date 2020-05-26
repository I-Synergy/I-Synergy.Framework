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
    public class MaxConcurrentRequestsMiddleware
    {
        private int _concurrentRequestsCount;

        private readonly RequestDelegate _next;
        private readonly MaxConcurrentRequestsOptions _options;
        private readonly MaxConcurrentRequestsEnqueuer _enqueuer;

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

        private async Task<bool> TryWaitInQueueAsync(CancellationToken requestAbortedCancellationToken)
        {
            return (_enqueuer != null) && (await _enqueuer.EnqueueAsync(requestAbortedCancellationToken).ConfigureAwait(false));
        }

        private async Task<bool> ShouldDecrementConcurrentRequestsCountAsync()
        {
            return (_options.Limit != MaxConcurrentRequestsOptions.ConcurrentRequestsUnlimited)
                && ((_enqueuer == null) || !(await _enqueuer.DequeueAsync().ConfigureAwait(false)));
        }
    }
}
