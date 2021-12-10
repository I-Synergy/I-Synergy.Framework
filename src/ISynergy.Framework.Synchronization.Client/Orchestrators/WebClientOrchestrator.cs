﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Synchronization.Client.Arguments;
using ISynergy.Framework.Synchronization.Client.Providers;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Database;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Setup;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client.Orchestrators
{
    public class WebClientOrchestrator : RemoteOrchestrator
    {

        /// <summary>
        /// Even if web client is acting as a proxy remote orchestrator, we are using it on the client side
        /// </summary>
        public override SyncSide Side => SyncSide.ClientSide;

        private readonly HttpRequestHandler _httpRequestHandler;
        private readonly IVersionService _versionService;

        public Dictionary<string, string> CustomHeaders => _httpRequestHandler.CustomHeaders;
        public Dictionary<string, string> ScopeParameters => _httpRequestHandler.ScopeParameters;

        private object locker = new object();

        /// <summary>
        /// Gets or Sets custom converter for all rows
        /// </summary>
        public IConverter Converter { get; set; }
        public int MaxDownladingDegreeOfParallelism { get; }


        /// <summary>
        /// Gets or Sets a custom sync policy
        /// </summary>
        public SyncPolicy SyncPolicy { get; set; }

        /// <summary>
        /// Gets or Sets the service uri used to reach the server api.
        /// </summary>
        public string ServiceUri { get; set; }

        /// <summary>
        /// Gets or Sets the HttpClient instanced used for this web client orchestrator
        /// </summary>
        public HttpClient HttpClient { get; set; }


        public string GetServiceHost()
        {
            if(string.IsNullOrEmpty(ServiceUri))
                return "Undefined";

            return new Uri(ServiceUri).Host;
        }

        /// <summary>
        /// Sets the current context
        /// </summary>
        internal override void SetContext(SyncContext context)
        {
            // we get a different reference from the web server,
            // so we copy the properties to the correct reference object
            var ctx = this.GetContext();

            context.CopyTo(ctx);
        }

        /// <summary>
        /// Gets a new web proxy orchestrator
        /// </summary>
        public WebClientOrchestrator(
            IVersionService versionService,
            string serviceUri,
            IConverter customConverter = null,
            HttpClient client = null,
            SyncPolicy syncPolicy = null,
            int maxDownladingDegreeOfParallelism = 4)
            : base(versionService, new ClientCoreProvider(), new SyncOptions(), new SyncSetup())
        {
            _versionService = versionService;
            _httpRequestHandler = new HttpRequestHandler(this);

            // if no HttpClient provisionned, create a new one
            if (client is null)
            {
                var handler = new HttpClientHandler();

                // Activated by default
                if (handler.SupportsAutomaticDecompression)
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                HttpClient = new HttpClient(handler);
            }
            else
            {
                HttpClient = client;
            }

            SyncPolicy = EnsurePolicy(syncPolicy);
            Converter = customConverter;
            MaxDownladingDegreeOfParallelism = maxDownladingDegreeOfParallelism <= 0 ? -1 : maxDownladingDegreeOfParallelism;
            ServiceUri = serviceUri;
        }

        /// <summary>
        /// Adds some scope parameters
        /// </summary>
        public void AddScopeParameter(string key, string value)
        {
            if (_httpRequestHandler.ScopeParameters.ContainsKey(key))
                _httpRequestHandler.ScopeParameters[key] = value;
            else
                _httpRequestHandler.ScopeParameters.Add(key, value);

        }

        /// <summary>
        /// Adds some custom headers
        /// </summary>
        public void AddCustomHeader(string key, string value)
        {
            if (_httpRequestHandler.CustomHeaders.ContainsKey(key))
                _httpRequestHandler.CustomHeaders[key] = value;
            else
                _httpRequestHandler.CustomHeaders.Add(key, value);

        }


        /// <summary>
        /// Get the schema from server, by sending an http request to the server
        /// </summary>
        public override async Task<SyncSet> GetSchemaAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            var serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);

            return serverScopeInfo.Schema;

        }

        public override Task<bool> IsOutDatedAsync(ScopeInfo clientScopeInfo, ServerScopeInfo serverScopeInfo, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
            => base.IsOutDatedAsync(clientScopeInfo, serverScopeInfo, cancellationToken, progress);

        /// <summary>
        /// Get server scope from server, by sending an http request to the server 
        /// </summary>
        public override async Task<ServerScopeInfo> GetServerScopeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get context or create a new one
            var ctx = this.GetContext();
            ctx.SyncStage = SyncStage.ScopeLoading;

            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            // Create the message to be sent
            var httpMessage = new HttpMessageEnsureScopesRequest(ctx);

            // serialize message
            var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>();
            var binaryData = await serializer.SerializeAsync(httpMessage);

            // Raise progress for sending request and waiting server response
            var sendingRequestArgs = new HttpGettingScopeRequestArgs(ctx, GetServiceHost());
            await this.InterceptAsync(sendingRequestArgs, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, sendingRequestArgs);

            // No batch size submitted here, because the schema will be generated in memory and send back to the user.
            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.EnsureScopes, ctx.SessionId, this.ScopeName,
                 this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageEnsureScopesResponse ensureScopesResponse = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (streamResponse.CanRead)
                    ensureScopesResponse = await this.Options.SerializerFactory.GetSerializer<HttpMessageEnsureScopesResponse>().DeserializeAsync(streamResponse);
            }

            if (ensureScopesResponse is null)
                throw new ArgumentException("Http Message content for Ensure scope can't be null");

            if (ensureScopesResponse.ServerScopeInfo is null)
                throw new ArgumentException("Server scope from EnsureScopesAsync can't be null and may contains a server scope");

            // Affect local setup
            this.Setup = ensureScopesResponse.ServerScopeInfo.Setup;

            // Reaffect context
            SetContext(ensureScopesResponse.SyncContext);

            // Report Progress
            var args = new HttpGettingScopeResponseArgs(ensureScopesResponse.ServerScopeInfo, ensureScopesResponse.SyncContext, GetServiceHost());
            await this.InterceptAsync(args, cancellationToken).ConfigureAwait(false);


            // Return scopes and new shema
            return ensureScopesResponse.ServerScopeInfo;
        }

        /// <summary>
        /// Send a request to remote web proxy for First step : Ensure scopes and schema
        /// </summary>
        internal override async Task<ServerScopeInfo> EnsureSchemaAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get context or create a new one
            var ctx = this.GetContext();
            ctx.SyncStage = SyncStage.SchemaReading;

            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            // Create the message to be sent
            var httpMessage = new HttpMessageEnsureScopesRequest(ctx);

            // serialize message
            var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>();
            var binaryData = await serializer.SerializeAsync(httpMessage);

            // Raise progress for sending request and waiting server response
            var sendingRequestArgs = new HttpGettingSchemaRequestArgs(ctx, GetServiceHost());
            await this.InterceptAsync(sendingRequestArgs, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, sendingRequestArgs);

            // No batch size submitted here, because the schema will be generated in memory and send back to the user.
            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.EnsureSchema, ctx.SessionId, this.ScopeName,
                 this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageEnsureSchemaResponse ensureScopesResponse = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (streamResponse.CanRead)
                    ensureScopesResponse = await this.Options.SerializerFactory.GetSerializer<HttpMessageEnsureSchemaResponse>().DeserializeAsync(streamResponse);
            }

            if (ensureScopesResponse is null)
                throw new ArgumentException("Http Message content for Ensure Schema can't be null");

            if (ensureScopesResponse.ServerScopeInfo is null || ensureScopesResponse.Schema is null || ensureScopesResponse.Schema.Tables.Count <= 0)
                throw new ArgumentException("Schema from EnsureScope can't be null and may contains at least one table");

            ensureScopesResponse.Schema.EnsureSchema();
            ensureScopesResponse.ServerScopeInfo.Schema = ensureScopesResponse.Schema;

            // Affect local setup
            this.Setup = ensureScopesResponse.ServerScopeInfo.Setup;

            // Reaffect context
            SetContext(ensureScopesResponse.SyncContext);

            // Report progress
            var args = new HttpGettingSchemaResponseArgs(ensureScopesResponse.ServerScopeInfo, ensureScopesResponse.Schema, ensureScopesResponse.SyncContext, GetServiceHost());
            await this.InterceptAsync(args, cancellationToken).ConfigureAwait(false);

            // Return scopes and new shema
            return ensureScopesResponse.ServerScopeInfo;
        }

        /// <summary>
        /// Apply changes
        /// </summary>
        internal override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, ConflictResolutionPolicy ServerPolicy,
                                      DatabaseChangesApplied ClientChangesApplied, DatabaseChangesSelected ServerChangesSelected)>
            ApplyThenGetChangesAsync(
                ScopeInfo scope, 
                BatchInfo clientBatchInfo, 
                CancellationToken cancellationToken = default, 
                IProgress<ProgressArgs> progress = null)
        {
            SyncSet schema;
            // Get context or create a new one
            var ctx = this.GetContext();

            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            // is it something that could happens ?
            if (scope.Schema is null)
            {
                // Make a remote call to get Schema from remote provider
                var serverScopeInfo = await EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);
                schema = serverScopeInfo.Schema;
            }
            else
            {
                schema = scope.Schema;
            }

            schema.EnsureSchema();

            ctx.SyncStage = SyncStage.ChangesApplying;

            // if we don't have any BatchPartsInfo, just generate a new one to get, at least, something to send to the server
            // and get a response with new data from server
            if (clientBatchInfo is null)
                clientBatchInfo = new BatchInfo(true, schema);

            // Get sanitized schema, without readonly columns
            var sanitizedSchema = clientBatchInfo.SanitizedSchema;

            // --------------------------------------------------------------
            // STEP 1 : Send everything to the server side
            // --------------------------------------------------------------


            HttpResponseMessage response = new HttpResponseMessage();

            // If not in memory and BatchPartsInfo.Count == 0, nothing to send.
            // But we need to send something, so generate a little batch part
            if (clientBatchInfo.InMemory || !clientBatchInfo.InMemory && clientBatchInfo.BatchPartsInfo.Count == 0)
            {
                var changesToSend = new HttpMessageSendChangesRequest(ctx, scope);

                if (Converter != null && clientBatchInfo.InMemoryData != null && clientBatchInfo.InMemoryData.HasRows)
                    this.BeforeSerializeRows(clientBatchInfo.InMemoryData);

                var containerSet = clientBatchInfo.InMemoryData is null ? new ContainerSet() : clientBatchInfo.InMemoryData.GetContainerSet();
                changesToSend.Changes = containerSet;
                changesToSend.IsLastBatch = true;
                changesToSend.BatchIndex = 0;
                changesToSend.BatchCount = clientBatchInfo.InMemoryData is null ? 0 : clientBatchInfo.BatchPartsInfo is null ? 0 : clientBatchInfo.BatchPartsInfo.Count;
                var inMemoryRowsCount = changesToSend.Changes.RowsCount();

                ctx.ProgressPercentage += 0.125;

                var args2 = new HttpSendingClientChangesRequestArgs(changesToSend, inMemoryRowsCount, inMemoryRowsCount, GetServiceHost());
                await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args2);

                // serialize message
                var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
                var binaryData = await serializer.SerializeAsync(changesToSend);

                response = await _httpRequestHandler.ProcessRequestAsync
                    (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, scope.Name,
                     this.Options.SerializerFactory, Converter, this.Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            }
            else
            {
                int tmpRowsSendedCount = 0;

                // Foreach part, will have to send them to the remote
                // once finished, return context
                var initialPctProgress1 = ctx.ProgressPercentage;
                foreach (var bpi in clientBatchInfo.BatchPartsInfo.OrderBy(bpi => bpi.Index))
                {
                    // If BPI is InMempory, no need to deserialize from disk
                    // othewise load it
                    await bpi.LoadBatchAsync(sanitizedSchema, clientBatchInfo.GetDirectoryFullPath(), this.Options.SerializerFactory, this);

                    var changesToSend = new HttpMessageSendChangesRequest(ctx, scope);

                    if (Converter != null && bpi.Data.HasRows)
                        BeforeSerializeRows(bpi.Data);

                    // Set the change request properties
                    changesToSend.Changes = bpi.Data.GetContainerSet();
                    changesToSend.IsLastBatch = bpi.IsLastBatch;
                    changesToSend.BatchIndex = bpi.Index;
                    changesToSend.BatchCount = clientBatchInfo.BatchPartsInfo.Count;

                    tmpRowsSendedCount += changesToSend.Changes.RowsCount();

                    ctx.ProgressPercentage = initialPctProgress1 + (changesToSend.BatchIndex + 1) * 0.2d / changesToSend.BatchCount;
                    var args2 = new HttpSendingClientChangesRequestArgs(changesToSend, tmpRowsSendedCount, clientBatchInfo.RowsCount, GetServiceHost());
                    await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);
                    this.ReportProgress(ctx, progress, args2);

                    // serialize message
                    var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
                    var binaryData = await serializer.SerializeAsync(changesToSend);

                    response = await _httpRequestHandler.ProcessRequestAsync
                        (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, scope.Name,
                         this.Options.SerializerFactory, Converter, this.Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // --------------------------------------------------------------
            // STEP 2 : Receive everything from the server side
            // --------------------------------------------------------------

            // Now we have sent all the datas to the server and now :
            // We have a FIRST response from the server with new datas 
            // 1) Could be the only one response (enough or InMemory is set on the server side)
            // 2) Could bt the first response and we need to download all batchs

            ctx.SyncStage = SyncStage.ChangesSelecting;
            var initialPctProgress = 0.55;
            ctx.ProgressPercentage = initialPctProgress;

            // Get if we need to work in memory or serialize things
            var workInMemoryLocally = this.Options.BatchSize == 0;

            // Create the BatchInfo
            var serverBatchInfo = new BatchInfo(workInMemoryLocally, schema);

            HttpMessageSummaryResponse summaryResponseContent = null;

            // Deserialize response incoming from server
            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();
                summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);
            }

            serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo.RowsCount;
            serverBatchInfo.Timestamp = summaryResponseContent.RemoteClientTimestamp;

            if (summaryResponseContent.BatchInfo.BatchPartsInfo != null)
                foreach (var bpi in summaryResponseContent.BatchInfo.BatchPartsInfo)
                    serverBatchInfo.BatchPartsInfo.Add(bpi);


            //-----------------------
            // In Memory Mode
            //-----------------------
            // response contains the rows because we are in memory mode
            if (summaryResponseContent.Changes != null && workInMemoryLocally)
            {
                var changesSet = new SyncSet();

                foreach (var tbl in summaryResponseContent.Changes.Tables)
                    DbSyncAdapter.CreateChangesTable(serverBatchInfo.SanitizedSchema.Tables[tbl.TableName, tbl.SchemaName], changesSet);

                changesSet.ImportContainerSet(summaryResponseContent.Changes, false);


                if (Converter != null)
                    AfterDeserializedRows(changesSet);

                // Create a BatchPartInfo instance
                await serverBatchInfo.AddChangesAsync(changesSet, 0, true, this.Options.SerializerFactory, this);

                // Raise response from server containing one finale batch changes 
                var args3 = new HttpGettingServerChangesResponseArgs(serverBatchInfo, 0, serverBatchInfo.RowsCount, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args3, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args3);

                return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ConflictResolutionPolicy,
                     summaryResponseContent.ClientChangesApplied, summaryResponseContent.ServerChangesSelected);
            }

            //-----------------------
            // In Batch Mode
            //-----------------------
            // From here, we need to serialize everything on disk

            // Generate the batch directory
            var batchDirectoryRoot = this.Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));

            serverBatchInfo.DirectoryRoot = batchDirectoryRoot;
            serverBatchInfo.DirectoryName = batchDirectoryName;

            //// hook to get the last batch part info at the end
            //var bpis = serverBatchInfo.BatchPartsInfo.Where(bpi => !bpi.IsLastBatch);
            //var lstbpi = serverBatchInfo.BatchPartsInfo.FirstOrDefault(bpi => bpi.IsLastBatch);

            // If we have a snapshot we are raising the batches downloading process that will occurs
            var args1 = new HttpBatchesDownloadingArgs(syncContext, this.StartTime.Value, serverBatchInfo, GetServiceHost());
            await this.InterceptAsync(args1, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args1);

            // function used to download one part
            var dl = new Func<BatchPartInfo, Task>(async (bpi) =>
            {
                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);
                var serializer3 = this.Options.SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3).ConfigureAwait(false);
                var step3 = HttpStep.GetMoreChanges;

                var args2 = new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);

                // Raise get changes request
                ctx.ProgressPercentage = initialPctProgress + (bpi.Index + 1) * 0.2d / serverBatchInfo.BatchPartsInfo.Count;

                var response = await _httpRequestHandler.ProcessRequestAsync(
                HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, this.ScopeName,
                this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                // Serialize
                await SerializeAsync(response, bpi.FileName, serverBatchInfo.GetDirectoryFullPath(), this).ConfigureAwait(false);

                bpi.SerializedType = typeof(BatchPartInfo);

                // Raise response from server containing a batch changes 
                var args3 = new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args3, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args3);
            });

            // Parrallel download of all bpis except the last one (which will launch the delete directory on the server side)
            await serverBatchInfo.BatchPartsInfo.ForEachAsync(bpi => dl(bpi), MaxDownladingDegreeOfParallelism).ConfigureAwait(false);

            // Send order of end of download
            var lastBpi = serverBatchInfo.BatchPartsInfo.FirstOrDefault(bpi => bpi.IsLastBatch);

            if (lastBpi != null)
            {
                var endOfDownloadChanges = new HttpMessageGetMoreChangesRequest(ctx, lastBpi.Index);
                var serializerEndOfDownloadChanges = this.Options.SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializerEndOfDownloadChanges.SerializeAsync(endOfDownloadChanges).ConfigureAwait(false);

                await _httpRequestHandler.ProcessRequestAsync(
                    HttpClient, ServiceUri, binaryData3, HttpStep.SendEndDownloadChanges, ctx.SessionId, this.ScopeName,
                    this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);
            }

            // generate the new scope item
            this.CompleteTime = DateTime.UtcNow;

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            var args4 = new HttpBatchesDownloadedArgs(summaryResponseContent, summaryResponseContent.SyncContext, this.StartTime.Value, DateTime.UtcNow, GetServiceHost());
            await this.InterceptAsync(args4, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args4);

            return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ConflictResolutionPolicy,
                    summaryResponseContent.ClientChangesApplied, summaryResponseContent.ServerChangesSelected);
        }


        public override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected DatabaseChangesSelected)>
          GetSnapshotAsync(SyncSet schema = null, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            // Get context or create a new one
            var ctx = this.GetContext();

            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            // Make a remote call to get Schema from remote provider
            if (schema is null)
            {
                var serverScopeInfo = await EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);
                schema = serverScopeInfo.Schema;
                schema.EnsureSchema();
            }

            ctx.SyncStage = SyncStage.SnapshotApplying;

            // Generate a batch directory
            var batchDirectoryRoot = this.Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));
            var batchDirectoryFullPath = Path.Combine(batchDirectoryRoot, batchDirectoryName);

            // Create the BatchInfo serialized (forced because in a snapshot call, so we are obviously serialized on disk)
            var serverBatchInfo = new BatchInfo(false, schema, batchDirectoryRoot, batchDirectoryName);

            // Firstly, get the snapshot summary
            var changesToSend = new HttpMessageSendChangesRequest(ctx, null);
            var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);
            var step = HttpStep.GetSummary;

            var response0 = await _httpRequestHandler.ProcessRequestAsync(
              HttpClient, ServiceUri, binaryData, step, ctx.SessionId, this.ScopeName,
              this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageSummaryResponse summaryResponseContent = null;

            using (var streamResponse = await response0.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();

                if (streamResponse.CanRead)
                {
                    summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);

                    serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo?.RowsCount ?? 0;

                    if (summaryResponseContent.BatchInfo?.BatchPartsInfo != null)
                        foreach (var bpi in summaryResponseContent.BatchInfo.BatchPartsInfo)
                            serverBatchInfo.BatchPartsInfo.Add(bpi);
                }

            }

            if (summaryResponseContent is null)
                throw new Exception("Summary can't be null");

            // no snapshot
            if ((serverBatchInfo.BatchPartsInfo is null || serverBatchInfo.BatchPartsInfo.Count <= 0) && serverBatchInfo.RowsCount <= 0)
                return (0, null, new DatabaseChangesSelected());

            // If we have a snapshot we are raising the batches downloading process that will occurs
            var args1 = new HttpBatchesDownloadingArgs(syncContext, this.StartTime.Value, serverBatchInfo, GetServiceHost());
            await this.InterceptAsync(args1, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args1);

            await serverBatchInfo.BatchPartsInfo.ForEachAsync(async bpi =>
            {
                // Create the message enveloppe
                Debug.WriteLine($"CLIENT bpi.FileName:{bpi.FileName}. bpi.Index:{bpi.Index}");

                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);
                var serializer3 = this.Options.SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3);
                var step3 = HttpStep.GetMoreChanges;

                var args2 = new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);

                var response = await _httpRequestHandler.ProcessRequestAsync(
                  HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, this.ScopeName,
                  this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                // Serialize
                await SerializeAsync(response, bpi.FileName, batchDirectoryFullPath, this);

                bpi.SerializedType = typeof(BatchPartInfo);

                // Raise response from server containing a batch changes 
                var args3 = new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args3, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args3);

            }, MaxDownladingDegreeOfParallelism);

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            var args4 = new HttpBatchesDownloadedArgs(summaryResponseContent, summaryResponseContent.SyncContext, this.StartTime.Value, DateTime.UtcNow, GetServiceHost());
            await this.InterceptAsync(args4, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args4);

            return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ServerChangesSelected);
        }

        /// <summary>
        /// Not Allowed from WebClientOrchestrator
        /// </summary>
        public override Task<DatabaseMetadatasCleaned> DeleteMetadatasAsync(long? timeStampStart, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
            => throw new NotImplementedException();

        /// <summary>
        /// Not Allowed from WebClientOrchestrator
        /// </summary>
        public override Task<bool> NeedsToUpgradeAsync(DbConnection connection = null, DbTransaction transaction = null, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
                        => throw new NotImplementedException();

        /// <summary>
        /// Not Allowed from WebClientOrchestrator
        /// </summary>
        public override Task<bool> UpgradeAsync(DbConnection connection = null, DbTransaction transaction = null, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
                        => throw new NotImplementedException();


        /// <summary>
        /// We can't get changes from server, from a web client orchestrator
        /// </summary>
        public override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected ServerChangesSelected)>
                                GetChangesAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {


            SyncSet schema;
            // Get context or create a new one
            var ctx = this.GetContext();

            // Get if we need to work in memory or serialize things
            var workInMemoryLocally = this.Options.BatchSize == 0;


            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            ServerScopeInfo serverScopeInfo;

            // Need the server scope
            serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            schema = serverScopeInfo.Schema;
            schema.EnsureSchema();

            clientScope.Schema = schema;
            clientScope.Setup = serverScopeInfo.Setup;
            clientScope.Version = serverScopeInfo.Version;

            var changesToSend = new HttpMessageSendChangesRequest(ctx, clientScope);

            var containerSet = new ContainerSet();
            changesToSend.Changes = containerSet;
            changesToSend.IsLastBatch = true;
            changesToSend.BatchIndex = 0;
            changesToSend.BatchCount = 0;

            ctx.ProgressPercentage += 0.125;

            var args2 = new HttpSendingClientChangesRequestArgs(changesToSend, 0, 0, GetServiceHost());
            await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, args2);

            // serialize message
            var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);

            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, clientScope.Name,
                 this.Options.SerializerFactory, Converter, this.Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);



            // --------------------------------------------------------------
            // STEP 2 : Receive everything from the server side
            // --------------------------------------------------------------

            // Now we have sent all the datas to the server and now :
            // We have a FIRST response from the server with new datas 
            // 1) Could be the only one response (enough or InMemory is set on the server side)
            // 2) Could bt the first response and we need to download all batchs

            ctx.SyncStage = SyncStage.ChangesSelecting;
            var initialPctProgress = 0.55;
            ctx.ProgressPercentage = initialPctProgress;


            // Create the BatchInfo
            var serverBatchInfo = new BatchInfo(workInMemoryLocally, schema);

            HttpMessageSummaryResponse summaryResponseContent = null;

            // Deserialize response incoming from server
            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();
                summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);
            }

            serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo.RowsCount;
            serverBatchInfo.Timestamp = summaryResponseContent.RemoteClientTimestamp;

            if (summaryResponseContent.BatchInfo.BatchPartsInfo != null)
                foreach (var bpi in summaryResponseContent.BatchInfo.BatchPartsInfo)
                    serverBatchInfo.BatchPartsInfo.Add(bpi);


            //-----------------------
            // In Memory Mode
            //-----------------------
            // response contains the rows because we are in memory mode
            if (summaryResponseContent.Changes != null && workInMemoryLocally)
            {
                var changesSet = new SyncSet();

                foreach (var tbl in summaryResponseContent.Changes.Tables)
                    DbSyncAdapter.CreateChangesTable(serverBatchInfo.SanitizedSchema.Tables[tbl.TableName, tbl.SchemaName], changesSet);

                changesSet.ImportContainerSet(summaryResponseContent.Changes, false);

                if (Converter != null)
                    AfterDeserializedRows(changesSet);

                // Create a BatchPartInfo instance
                await serverBatchInfo.AddChangesAsync(changesSet, 0, true, this.Options.SerializerFactory, this);

                // Raise response from server containing one finale batch changes 
                var args3 = new HttpGettingServerChangesResponseArgs(serverBatchInfo, 0, serverBatchInfo.RowsCount, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args3, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args3);

                return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ServerChangesSelected);
            }

            //-----------------------
            // In Batch Mode
            //-----------------------
            // From here, we need to serialize everything on disk

            // Generate the batch directory
            var batchDirectoryRoot = this.Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));

            serverBatchInfo.DirectoryRoot = batchDirectoryRoot;
            serverBatchInfo.DirectoryName = batchDirectoryName;

            // hook to get the last batch part info at the end
            var bpis = serverBatchInfo.BatchPartsInfo.Where(bpi => !bpi.IsLastBatch);
            var lstbpi = serverBatchInfo.BatchPartsInfo.First(bpi => bpi.IsLastBatch);

            // function used to download one part
            var dl = new Func<BatchPartInfo, Task>(async (bpi) =>
            {
                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);
                var serializer3 = this.Options.SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3).ConfigureAwait(false);
                var step3 = HttpStep.GetMoreChanges;

                var args2 = new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args2, cancellationToken).ConfigureAwait(false);

                // Raise get changes request
                ctx.ProgressPercentage = initialPctProgress + (bpi.Index + 1) * 0.2d / serverBatchInfo.BatchPartsInfo.Count;

                var response = await _httpRequestHandler.ProcessRequestAsync(
                HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, this.ScopeName,
                this.Options.SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                // Serialize
                await SerializeAsync(response, bpi.FileName, serverBatchInfo.GetDirectoryFullPath(), this).ConfigureAwait(false);

                bpi.SerializedType = typeof(BatchPartInfo);

                // Raise response from server containing a batch changes 
                var args3 = new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost());
                await this.InterceptAsync(args3, cancellationToken).ConfigureAwait(false);
                this.ReportProgress(ctx, progress, args3);
            });

            // Parrallel download of all bpis except the last one (which will launch the delete directory on the server side)
            await bpis.ForEachAsync(bpi => dl(bpi), MaxDownladingDegreeOfParallelism).ConfigureAwait(false);

            // Download last batch part that will launch the server deletion of the tmp dir
            await dl(lstbpi).ConfigureAwait(false);

            // generate the new scope item
            this.CompleteTime = DateTime.UtcNow;

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ServerChangesSelected);
        }



        /// <summary>
        /// We can't get changes from server, from a web client orchestrator
        /// </summary>
        public override async Task<(long RemoteClientTimestamp, DatabaseChangesSelected ServerChangesSelected)>
                                GetEstimatedChangesCountAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            SyncSet schema;
            // Get context or create a new one
            var ctx = this.GetContext();

            if (!this.StartTime.HasValue)
                this.StartTime = DateTime.UtcNow;

            ServerScopeInfo serverScopeInfo;

            // Need the server scope
            serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);
            schema = serverScopeInfo.Schema;
            schema.EnsureSchema();

            clientScope.Schema = schema;
            clientScope.Setup = serverScopeInfo.Setup;
            clientScope.Version = serverScopeInfo.Version;


            // generate a message to send
            var changesToSend = new HttpMessageSendChangesRequest(ctx, clientScope)
            {
                Changes = null,
                IsLastBatch = true,
                BatchIndex = 0,
                BatchCount = 0
            };

            var serializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);

            // Raise progress for sending request and waiting server response
            var requestArgs = new HttpGettingServerChangesRequestArgs(0, 0, ctx, GetServiceHost());
            await this.InterceptAsync(requestArgs, cancellationToken).ConfigureAwait(false);
            this.ReportProgress(ctx, progress, requestArgs);

            // response
            var response = await _httpRequestHandler.ProcessRequestAsync
                    (HttpClient, ServiceUri, binaryData, HttpStep.GetEstimatedChangesCount, ctx.SessionId, clientScope.Name,
                     this.Options.SerializerFactory, Converter, this.Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageSendChangesResponse summaryResponseContent = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = this.Options.SerializerFactory.GetSerializer<HttpMessageSendChangesResponse>();

                if (streamResponse.CanRead)
                    summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);

            }

            if (summaryResponseContent is null)
                throw new Exception("Summary can't be null");

            // generate the new scope item
            this.CompleteTime = DateTime.UtcNow;

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            return (summaryResponseContent.RemoteClientTimestamp, summaryResponseContent.ServerChangesSelected);
        }



        public void BeforeSerializeRows(SyncSet data)
        {
            foreach (var table in data.Tables)
            {
                if (table.Rows.Count > 0)
                {
                    foreach (var row in table.Rows)
                        Converter.BeforeSerialize(row);

                }
            }
        }

        public void AfterDeserializedRows(SyncSet data)
        {
            foreach (var table in data.Tables)
            {
                if (table.Rows.Count > 0)
                {
                    foreach (var row in table.Rows)
                        Converter.AfterDeserialized(row);

                }
            }

        }

        /// <summary>
        /// Ensure we have policy. Create a new one, if not provided
        /// </summary>
        private SyncPolicy EnsurePolicy(SyncPolicy policy)
        {
            if (policy != default)
                return policy;

            // Defining my retry policy
            policy = SyncPolicy.WaitAndRetry(10,
            (retryNumber) =>
            {
                return TimeSpan.FromMilliseconds(500 * retryNumber);
            },
            (ex, arg) =>
            {
                var webEx = ex as SyncException;

                // handle session lost
                return webEx is null || webEx.TypeName != nameof(HttpSessionLostException);

            }, async (ex, cpt, ts, arg) =>
            {
                SyncContext syncContext = this.GetContext();
                IProgress<ProgressArgs> progressArgs = arg as IProgress<ProgressArgs>;
                var args = new HttpSyncPolicyArgs(syncContext, 10, cpt, ts);
                await this.InterceptAsync(args, default).ConfigureAwait(false);
                this.ReportProgress(syncContext, progressArgs, args, null, null);
            });


            return policy;

        }

        ///// <summary>
        ///// Extract DMS Headers
        ///// </summary>
        //private static async Task<HttpHeaderInfo> GetDmsHeaders(HttpResponseMessage response)
        //{
        //    HttpHeaderInfo httpHeaderInfo = null;

        //    if (response.Headers != null)
        //    {
        //        if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync", out string dmsStringEncodedString))
        //        {
        //            var dmsStringEncodedArray = Convert.FromBase64String(dmsStringEncodedString);
        //            var dmsString = Encoding.UTF8.GetString(dmsStringEncodedArray);
        //            using var stringReader = new StringReader(dmsString);
        //            using var jreader = new JsonTextReader(stringReader);
        //            var jobject = await JObject.LoadAsync(jreader);

        //            httpHeaderInfo = jobject.ToObject<HttpHeaderInfo>();
        //        }
        //        else
        //        {
        //            httpHeaderInfo = new HttpHeaderInfo();

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-islb", out string isLastBatchString))
        //                httpHeaderInfo.IsLastBatch = SyncTypeConverter.TryConvertTo<bool>(isLastBatchString);

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-bi", out string isBatchIndexString))
        //                httpHeaderInfo.BatchIndex = SyncTypeConverter.TryConvertTo<int>(isBatchIndexString);

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-rows-count", out string rowsCountString))
        //                httpHeaderInfo.RowsCount = SyncTypeConverter.TryConvertTo<int>(rowsCountString);

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-bi-tables", out string tablesString))
        //                httpHeaderInfo.Tables = JsonConvert.DeserializeObject<BatchPartTableInfo[]>(tablesString);

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-bc", out string batchCountString))
        //                httpHeaderInfo.BatchCount = SyncTypeConverter.TryConvertTo<int>(batchCountString);

        //            if (HttpRequestHandler.TryGetHeaderValue(response.Headers, "dotmim-sync-context", out string syncContextString))
        //                httpHeaderInfo.SyncContext = JsonConvert.DeserializeObject<SyncContext>(syncContextString);
        //        }

        //    }

        //    return httpHeaderInfo;
        //}

        private static async Task SerializeAsync(HttpResponseMessage response, string fileName, string directoryFullPath, BaseOrchestrator orchestrator = null)
        {
            var fullPath = Path.Combine(directoryFullPath, fileName);

            var fi = new FileInfo(fullPath);

            if (!Directory.Exists(fi.Directory.FullName))
                Directory.CreateDirectory(fi.Directory.FullName);

            // Read response
            using var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.ReadWrite);

            await streamResponse.CopyToAsync(fileStream).ConfigureAwait(false);

        }

        private static async Task<HttpMessageSendChangesResponse> DeserializeAsync(ISerializerFactory serializerFactory, string fileName, string directoryFullPath, BaseOrchestrator orchestrator = null)
        {
            var fullPath = Path.Combine(directoryFullPath, fileName);
            using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var httpMessageContent = await serializerFactory.GetSerializer<HttpMessageSendChangesResponse>().DeserializeAsync(fileStream);
            return httpMessageContent;
        }

    }
}
