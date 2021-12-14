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

    /// <summary>
    /// Represents a request made to the server to get the server scope info
    /// </summary>
    public class HttpBatchesDownloadingArgs : ProgressArgs
    {
        public HttpBatchesDownloadingArgs(SyncContext context, DateTime startTime, BatchInfo serverBatchInfo, string host) : base(context, null)
        {
            StartTime = startTime;
            ServerBatchInfo = serverBatchInfo;
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Host;
        public override int EventId => HttpClientSyncEventsId.HttpGettingSchemaRequest.Id;
        public override string Message => $"Downloading Batches. Scope Name:{Context.ScopeName}. Batches Count:{ServerBatchInfo.BatchPartsInfo?.Count ?? 1}. Rows Count:{ServerBatchInfo.RowsCount}";

        public DateTime StartTime { get; }
        public BatchInfo ServerBatchInfo { get; }
        public string Host { get; }
    }
    public class HttpBatchesDownloadedArgs : ProgressArgs
    {
        public HttpBatchesDownloadedArgs(HttpMessageSummaryResponse httpSummary, SyncContext context, DateTime startTime, DateTime completeTime, string host) : base(context, null)
        {
            HttpSummary = httpSummary;
            StartTime = startTime;
            CompleteTime = completeTime;
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Host;
        public override int EventId => HttpClientSyncEventsId.HttpGettingSchemaResponse.Id;
        public override string Message
        {
            get
            {
                var batchCount = HttpSummary.BatchInfo?.BatchPartsInfo?.Count ?? 1;
                var totalRows = HttpSummary.ServerChangesSelected?.TotalChangesSelected ?? 0;

                return $"Snapshot Downloaded. Batches Count: {batchCount}. Total Rows: {totalRows}. Duration: {Duration:hh\\:mm\\:ss}";
            }
        }

        public HttpMessageSummaryResponse HttpSummary { get; }
        public DateTime StartTime { get; }
        public DateTime CompleteTime { get; }

        public TimeSpan Duration
        {
            get
            {
                var tsEnded = TimeSpan.FromTicks(CompleteTime.Ticks);
                var tsStarted = TimeSpan.FromTicks(StartTime.Ticks);

                var durationTs = tsEnded.Subtract(tsStarted);

                return durationTs;
            }
        }

        public string Host { get; }
    }

    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpBatchesDownloadingArgs => new EventId(20600, nameof(HttpBatchesDownloadingArgs));
        public static EventId HttpBatchesDownloadedArgs => new EventId(20650, nameof(HttpBatchesDownloadedArgs));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when batches are about to be downloaded
        /// </summary>
        public static void OnHttpBatchesDownloadingArgs(this WebClientOrchestrator orchestrator, Action<HttpBatchesDownloadingArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when batches are about to be downloaded
        /// </summary>
        public static void OnHttpBatchesDownloadingArgs(this WebClientOrchestrator orchestrator, Func<HttpBatchesDownloadingArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when batches have been completely downloaded
        /// </summary>
        public static void OnHttpBatchesDownloadedArgs(this WebClientOrchestrator orchestrator, Action<HttpBatchesDownloadedArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when batches have been completely downloaded
        /// </summary>
        public static void OnHttpBatchesDownloadedArgs(this WebClientOrchestrator orchestrator, Func<HttpBatchesDownloadedArgs, Task> action)
            => orchestrator.SetInterceptor(action);



    }
}
