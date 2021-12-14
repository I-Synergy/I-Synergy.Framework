using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client.Arguments
{

    public class HttpGettingServerChangesRequestArgs : ProgressArgs
    {
        public HttpGettingServerChangesRequestArgs(int batchIndexRequested, int batchCount, SyncContext context, string host)
             : base(context, null, null)
        {
            BatchIndexRequested = batchIndexRequested;
            BatchCount = batchCount;
            Host = host;
        }
        public override string Source => Host;
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Message
        {
            get
            {
                if (BatchCount <= 1)
                    return $"Getting Batch Changes.";
                else
                    return $"Getting Batch Changes. ({BatchIndexRequested + 1}/{BatchCount}).";
            }
        }

        /// <summary>
        /// Gets the batch index that is asked to be retrieved from the server
        /// </summary>
        public int BatchIndexRequested { get; set; }

        /// <summary>
        /// Gets the batch count to be received from server 
        /// </summary>
        public int BatchCount { get; set; }


        public string Host { get; }

        public override int EventId => HttpClientSyncEventsId.HttpGettingChangesRequest.Id;
    }

    public class HttpGettingServerChangesResponseArgs : ProgressArgs
    {
        public HttpGettingServerChangesResponseArgs(BatchInfo batchInfo, int batchIndex, int batchRowsCount, SyncContext syncContext, string host)
            : base(syncContext, null, null)
        {
            BatchInfo = batchInfo;
            BatchIndex = batchIndex;
            BatchRowsCount = batchRowsCount;
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Host;
        public override string Message
        {
            get
            {
                var batchesCount = BatchInfo.BatchPartsInfo?.Count ?? 1;
                return $"Downloaded Batch Changes. ({BatchIndex + 1}/{batchesCount}). Rows:({BatchRowsCount}/{BatchInfo.RowsCount}).";
            }
        }

        public BatchInfo BatchInfo { get; }
        public int BatchIndex { get; }
        public int BatchRowsCount { get; }
        public string Host { get; }

        public override int EventId => HttpClientSyncEventsId.HttpGettingChangesResponse.Id;
    }

    public class HttpSendingClientChangesRequestArgs : ProgressArgs
    {
        public HttpSendingClientChangesRequestArgs(HttpMessageSendChangesRequest request, int rowsCount, int totalRowsCount, string host)
            : base(request.SyncContext, null, null)
        {
            Request = request;
            RowsCount = rowsCount;
            TotalRowsCount = totalRowsCount;
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public HttpMessageSendChangesRequest Request { get; }
        public string Host { get; }
        public override string Source => Host;
        public override string Message
        {
            get
            {
                if (Request.BatchCount == 0 && Request.BatchIndex == 0)
                    return $"Sending All Changes. Rows:{RowsCount}. Waiting Server Response...";
                else
                    return $"Sending Batch Changes. Batches: ({Request.BatchIndex + 1}/{Request.BatchCount}). Rows: ({RowsCount}/{TotalRowsCount}). Waiting Server Response...";
            }
        }

        /// <summary>
        /// Gets or Sets the rows count sended
        /// </summary>
        public int RowsCount { get; set; }

        /// <summary>
        /// Gets or Sets the total tables rows count to send
        /// </summary>
        public int TotalRowsCount { get; set; }

        public override int EventId => HttpClientSyncEventsId.HttpSendingChangesRequest.Id;
    }


    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpSendingChangesRequest => new EventId(20000, nameof(HttpSendingChangesRequest));
        public static EventId HttpGettingChangesRequest => new EventId(20100, nameof(HttpGettingChangesRequest));
        public static EventId HttpGettingChangesResponse => new EventId(20150, nameof(HttpGettingChangesResponse));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when batch changes is uploading to server.
        /// </summary>
        public static void OnHttpSendingChangesRequest(this WebClientOrchestrator orchestrator, Action<HttpSendingClientChangesRequestArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when batch changes is uploading to server.
        /// </summary>
        public static void OnHttpSendingChangesRequest(this WebClientOrchestrator orchestrator, Func<HttpSendingClientChangesRequestArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when downloading a batch changes from server side.
        /// </summary>
        public static void OnHttpGettingChangesRequest(this WebClientOrchestrator orchestrator, Action<HttpGettingServerChangesRequestArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when downloading a batch changes from server side.
        /// </summary>
        public static void OnHttpGettingChangesRequest(this WebClientOrchestrator orchestrator, Func<HttpGettingServerChangesRequestArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a batch changes has been downloaded from server side
        /// </summary>
        public static void OnHttpGettingChangesResponse(this WebClientOrchestrator orchestrator, Action<HttpGettingServerChangesResponseArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when a batch changes has been downloaded from server side
        /// </summary>
        public static void OnHttpGettingChangesResponse(this WebClientOrchestrator orchestrator, Func<HttpGettingServerChangesResponseArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }
}
