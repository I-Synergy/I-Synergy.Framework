namespace ISynergy.Framework.AspNetCore.Middleware.Internals
{
    /// <summary>
    /// Class MaxConcurrentRequestsEnqueuer.
    /// </summary>
    internal class MaxConcurrentRequestsEnqueuer
    {
        /// <summary>
        /// Enum DropMode
        /// </summary>
        public enum DropMode
        {
            /// <summary>
            /// The tail
            /// </summary>
            Tail = MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropTail,
            /// <summary>
            /// The head
            /// </summary>
            Head = MaxConcurrentRequestsLimitExceededPolicy.FifoQueueDropHead
        }

        /// <summary>
        /// The queue semaphore
        /// </summary>
        private readonly SemaphoreSlim _queueSemaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The maximum queue length
        /// </summary>
        private readonly int _maxQueueLength;
        /// <summary>
        /// The drop mode
        /// </summary>
        private readonly DropMode _dropMode;
        /// <summary>
        /// The maximum time in queue
        /// </summary>
        private readonly int _maxTimeInQueue;

        /// <summary>
        /// The queue
        /// </summary>
        private readonly LinkedList<TaskCompletionSource<bool>> _queue = new LinkedList<TaskCompletionSource<bool>>();

        /// <summary>
        /// The enqueue failed task
        /// </summary>
        private static readonly Task<bool> _enqueueFailedTask = Task.FromResult(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxConcurrentRequestsEnqueuer"/> class.
        /// </summary>
        /// <param name="maxQueueLength">Maximum length of the queue.</param>
        /// <param name="dropMode">The drop mode.</param>
        /// <param name="maxTimeInQueue">The maximum time in queue.</param>
        public MaxConcurrentRequestsEnqueuer(int maxQueueLength, DropMode dropMode, int maxTimeInQueue)
        {
            _maxQueueLength = maxQueueLength;
            _dropMode = dropMode;
            _maxTimeInQueue = maxTimeInQueue;
        }

        /// <summary>
        /// enqueue as an asynchronous operation.
        /// </summary>
        /// <param name="requestAbortedCancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> EnqueueAsync(CancellationToken requestAbortedCancellationToken)
        {
            var enqueueTask = _enqueueFailedTask;

            if (_maxQueueLength > 0)
            {
                var enqueueCancellationToken = GetEnqueueCancellationToken(requestAbortedCancellationToken);

                try
                {
                    await _queueSemaphore.WaitAsync(enqueueCancellationToken).ConfigureAwait(false);
                    try
                    {
                        if (_queue.Count < _maxQueueLength)
                        {
                            enqueueTask = InternalEnqueueAsync(enqueueCancellationToken);
                        }
                        else if (_dropMode == DropMode.Head)
                        {
                            InternalDequeue(false);

                            enqueueTask = InternalEnqueueAsync(enqueueCancellationToken);
                        }
                    }
                    finally
                    {
                        _queueSemaphore.Release();
                    }
                }
                catch (OperationCanceledException)
                { }
            }

            return await enqueueTask.ConfigureAwait(false);
        }

        /// <summary>
        /// dequeue as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> DequeueAsync()
        {
            var dequeued = false;

            await _queueSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_queue.Count > 0)
                {
                    InternalDequeue(true);
                    dequeued = true;
                }
            }
            finally
            {
                _queueSemaphore.Release();
            }

            return dequeued;
        }

        /// <summary>
        /// Internals the enqueue asynchronous.
        /// </summary>
        /// <param name="enqueueCancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private Task<bool> InternalEnqueueAsync(CancellationToken enqueueCancellationToken)
        {
            var enqueueTask = _enqueueFailedTask;

            if (!enqueueCancellationToken.IsCancellationRequested)
            {
                var enqueueTaskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                enqueueCancellationToken.Register(CancelEnqueue, enqueueTaskCompletionSource);

                _queue.AddLast(enqueueTaskCompletionSource);
                enqueueTask = enqueueTaskCompletionSource.Task;
            }

            return enqueueTask;
        }

        /// <summary>
        /// Gets the enqueue cancellation token.
        /// </summary>
        /// <param name="requestAbortedCancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>CancellationToken.</returns>
        private CancellationToken GetEnqueueCancellationToken(CancellationToken requestAbortedCancellationToken)
        {
            var enqueueCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(
                requestAbortedCancellationToken,
                GetTimeoutToken()
            ).Token;

            return enqueueCancellationToken;
        }

        /// <summary>
        /// Gets the timeout token.
        /// </summary>
        /// <returns>CancellationToken.</returns>
        private CancellationToken GetTimeoutToken()
        {
            var timeoutToken = CancellationToken.None;

            if (_maxTimeInQueue != MaxConcurrentRequestsOptions.MaxTimeInQueueUnlimited)
            {
                var timeoutTokenSource = new CancellationTokenSource();

                timeoutToken = timeoutTokenSource.Token;

                timeoutTokenSource.CancelAfter(_maxTimeInQueue);
            }

            return timeoutToken;
        }

        /// <summary>
        /// Cancels the enqueue.
        /// </summary>
        /// <param name="state">The state.</param>
        private void CancelEnqueue(object state)
        {
            var removed = false;
            var enqueueTaskCompletionSource = ((TaskCompletionSource<bool>)state);

            // This is blocking, but it looks like this callback can't be asynchronous.
            _queueSemaphore.Wait();

            try
            {
                removed = _queue.Remove(enqueueTaskCompletionSource);
            }
            finally
            {
                _queueSemaphore.Release();
            }

            if (removed)
            {
                enqueueTaskCompletionSource.SetResult(false);
            }
        }

        /// <summary>
        /// Internals the dequeue.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        private void InternalDequeue(bool result)
        {
            var enqueueTaskCompletionSource = _queue.First.Value;

            _queue.RemoveFirst();

            enqueueTaskCompletionSource.SetResult(result);
        }
    }
}
