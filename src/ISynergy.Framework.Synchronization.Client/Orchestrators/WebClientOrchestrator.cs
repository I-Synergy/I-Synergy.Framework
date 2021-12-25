using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Synchronization.Client.Enumerations;
using ISynergy.Framework.Synchronization.Client.Handlers;
using ISynergy.Framework.Synchronization.Client.Messages;
using ISynergy.Framework.Synchronization.Client.Providers;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public Dictionary<string, string> CustomHeaders => _httpRequestHandler.CustomHeaders;
        public Dictionary<string, string> ScopeParameters => _httpRequestHandler.ScopeParameters;

        /// <summary>
        /// Gets or Sets custom converter for all rows
        /// </summary>
        public IConverter Converter { get; set; }

        /// <summary>
        /// Max threads used to get parts from server
        /// </summary>
        public int MaxDownladingDegreeOfParallelism { get; }

        /// <summary>
        /// Gets or Sets serializer used to serialize and deserialize rows coming from server
        /// </summary>
        public ISerializerFactory SerializerFactory { get; set; }
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
            if (string.IsNullOrEmpty(ServiceUri))
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
            var ctx = GetContext();

            context.CopyTo(ctx);
        }

        /// <summary>
        /// Gets a new web proxy orchestrator
        /// </summary>
        public WebClientOrchestrator(
            string serviceUri,
            IVersionService versionService,
            IConverter customConverter = null,
            HttpClient client = null,
            SyncPolicy syncPolicy = null,
            int maxDownladingDegreeOfParallelism = 4)
            : base(versionService, new ClientCoreProvider(), new SyncOptions(), new SyncSetup())
        {
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
            SerializerFactory = SerializersCollection.JsonSerializerFactory;
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
            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

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
            var ctx = GetContext();
            ctx.SyncStage = SyncStage.ScopeLoading;

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // Create the message to be sent
            var httpMessage = new HttpMessageEnsureScopesRequest(ctx);

            // serialize message
            var serializer = SerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>();
            var binaryData = await serializer.SerializeAsync(httpMessage);

            // Raise progress for sending request and waiting server response
            await InterceptAsync(new HttpGettingScopeRequestArgs(ctx, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // No batch size submitted here, because the schema will be generated in memory and send back to the user.
            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.EnsureScopes, ctx.SessionId, ScopeName,
                 SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageEnsureScopesResponse ensureScopesResponse = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (streamResponse.CanRead)
                    ensureScopesResponse = await SerializerFactory.GetSerializer<HttpMessageEnsureScopesResponse>().DeserializeAsync(streamResponse);
            }

            if (ensureScopesResponse is null)
                throw new ArgumentException("Http Message content for Ensure scope can't be null");

            if (ensureScopesResponse.ServerScopeInfo is null)
                throw new ArgumentException("Server scope from EnsureScopesAsync can't be null and may contains a server scope");

            // Affect local setup
            Setup = ensureScopesResponse.ServerScopeInfo.Setup;

            // Reaffect context
            SetContext(ensureScopesResponse.SyncContext);

            // Report Progress
            await InterceptAsync(new HttpGettingScopeResponseArgs(ensureScopesResponse.ServerScopeInfo, ensureScopesResponse.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);


            // Return scopes and new shema
            return ensureScopesResponse.ServerScopeInfo;
        }

        /// <summary>
        /// Send a request to remote web proxy for First step : Ensure scopes and schema
        /// </summary>
        internal override async Task<ServerScopeInfo> EnsureSchemaAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get context or create a new one
            var ctx = GetContext();
            ctx.SyncStage = SyncStage.SchemaReading;

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // Create the message to be sent
            var httpMessage = new HttpMessageEnsureScopesRequest(ctx);

            // serialize message
            var serializer = SerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>();
            var binaryData = await serializer.SerializeAsync(httpMessage);

            // Raise progress for sending request and waiting server response
            await InterceptAsync(new HttpGettingSchemaRequestArgs(ctx, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // No batch size submitted here, because the schema will be generated in memory and send back to the user.
            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.EnsureSchema, ctx.SessionId, ScopeName,
                 SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageEnsureSchemaResponse ensureScopesResponse = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                if (streamResponse.CanRead)
                    ensureScopesResponse = await SerializerFactory.GetSerializer<HttpMessageEnsureSchemaResponse>().DeserializeAsync(streamResponse);
            }

            if (ensureScopesResponse is null)
                throw new ArgumentException("Http Message content for Ensure Schema can't be null");

            if (ensureScopesResponse.ServerScopeInfo is null || ensureScopesResponse.Schema is null || ensureScopesResponse.Schema.Tables.Count <= 0)
                throw new ArgumentException("Schema from EnsureScope can't be null and may contains at least one table");

            ensureScopesResponse.Schema.EnsureSchema();
            ensureScopesResponse.ServerScopeInfo.Schema = ensureScopesResponse.Schema;

            // Affect local setup
            Setup = ensureScopesResponse.ServerScopeInfo.Setup;

            // Reaffect context
            SetContext(ensureScopesResponse.SyncContext);

            // Report progress
            await InterceptAsync(new HttpGettingSchemaResponseArgs(ensureScopesResponse.ServerScopeInfo, ensureScopesResponse.Schema, ensureScopesResponse.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // Return scopes and new shema
            return ensureScopesResponse.ServerScopeInfo;
        }

        /// <summary>
        /// Apply changes
        /// </summary>
        internal override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, ConflictResolutionPolicy ServerPolicy,
                                      DatabaseChangesApplied ClientChangesApplied, DatabaseChangesSelected ServerChangesSelected)>
            ApplyThenGetChangesAsync(ScopeInfo scope, BatchInfo clientBatchInfo, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            SyncSet schema;
            // Get context or create a new one
            var ctx = GetContext();

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // is it something that could happens ?
            if (scope.Schema is null)
            {
                // Make a remote call to get Schema from remote provider
                var serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);
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
                clientBatchInfo = new BatchInfo(schema);

            // --------------------------------------------------------------
            // STEP 1 : Send everything to the server side
            // --------------------------------------------------------------

            HttpResponseMessage response = null;

            // If not in memory and BatchPartsInfo.Count == 0, nothing to send.
            // But we need to send something, so generate a little batch part
            if (clientBatchInfo.BatchPartsInfo.Count == 0)
            {
                var changesToSend = new HttpMessageSendChangesRequest(ctx, scope);

                var containerSet = new ContainerSet();
                changesToSend.Changes = containerSet;
                changesToSend.IsLastBatch = true;
                changesToSend.BatchIndex = 0;
                changesToSend.BatchCount = clientBatchInfo.BatchPartsInfo is null ? 0 : clientBatchInfo.BatchPartsInfo.Count;
                var inMemoryRowsCount = changesToSend.Changes.RowsCount();

                ctx.ProgressPercentage += 0.125;

                await InterceptAsync(new HttpSendingClientChangesRequestArgs(changesToSend, inMemoryRowsCount, inMemoryRowsCount, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

                // serialize message
                var serializer = SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
                var binaryData = await serializer.SerializeAsync(changesToSend);

                response = await _httpRequestHandler.ProcessRequestAsync
                    (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, scope.Name,
                     SerializerFactory, Converter, Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            }
            else
            {
                int tmpRowsSendedCount = 0;

                // Foreach part, will have to send them to the remote
                // once finished, return context
                var initialPctProgress1 = ctx.ProgressPercentage;
                foreach (var bpi in clientBatchInfo.BatchPartsInfo.OrderBy(bpi => bpi.Index))
                {
                    // Get the updatable schema for the only table contained in the batchpartinfo
                    var schemaTable = DbSyncAdapter.CreateChangesTable(schema.Tables[bpi.Tables[0].TableName, bpi.Tables[0].SchemaName]);

                    // Generate the ContainerSet containing rows to send to the user
                    var containerSet = new ContainerSet();
                    var containerTable = new ContainerTable(schemaTable);
                    var fullPath = Path.Combine(clientBatchInfo.GetDirectoryFullPath(), bpi.FileName);
                    containerSet.Tables.Add(containerTable);

                    var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();
                    // read rows from file
                    foreach (var row in localSerializer.ReadRowsFromFile(fullPath, schemaTable))
                        containerTable.Rows.Add(row.ToArray());

                    // Call the converter if needed
                    if (Converter is not null && containerTable.HasRows)
                        BeforeSerializeRows(containerTable, schemaTable, Converter);

                    // Create the send changes request
                    var changesToSend = new HttpMessageSendChangesRequest(ctx, scope)
                    {
                        Changes = containerSet,
                        IsLastBatch = bpi.IsLastBatch,
                        BatchIndex = bpi.Index,
                        BatchCount = clientBatchInfo.BatchPartsInfo.Count
                    };

                    tmpRowsSendedCount += containerTable.Rows.Count;

                    ctx.ProgressPercentage = initialPctProgress1 + (changesToSend.BatchIndex + 1) * 0.2d / changesToSend.BatchCount;
                    await InterceptAsync(new HttpSendingClientChangesRequestArgs(changesToSend, tmpRowsSendedCount, clientBatchInfo.RowsCount, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

                    // serialize message
                    var serializer = SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
                    var binaryData = await serializer.SerializeAsync(changesToSend);

                    response = await _httpRequestHandler.ProcessRequestAsync
                        (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, scope.Name,
                         SerializerFactory, Converter, Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);
                }
            }

            // --------------------------------------------------------------
            // STEP 2 : Receive everything from the server side
            // --------------------------------------------------------------

            // Now we have sent all the datas to the server and now :
            // We have a FIRST response from the server with new datas 
            // 1) Could be the only one response 
            // 2) Could be the first response and we need to download all batchs

            ctx.SyncStage = SyncStage.ChangesSelecting;
            var initialPctProgress = 0.55;
            ctx.ProgressPercentage = initialPctProgress;

            // Create the BatchInfo
            var serverBatchInfo = new BatchInfo(schema);

            HttpMessageSummaryResponse summaryResponseContent = null;

            // Deserialize response incoming from server
            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();
                summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);
            }

            serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo.RowsCount;
            serverBatchInfo.Timestamp = summaryResponseContent.RemoteClientTimestamp;

            if (summaryResponseContent.BatchInfo.BatchPartsInfo is not null)
                foreach (var bpi in summaryResponseContent.BatchInfo.BatchPartsInfo)
                    serverBatchInfo.BatchPartsInfo.Add(bpi);


            // From here, we need to serialize everything on disk

            // Generate the batch directory
            var batchDirectoryRoot = Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));

            serverBatchInfo.DirectoryRoot = batchDirectoryRoot;
            serverBatchInfo.DirectoryName = batchDirectoryName;

            if (!Directory.Exists(serverBatchInfo.GetDirectoryFullPath()))
                Directory.CreateDirectory(serverBatchInfo.GetDirectoryFullPath());

            // If we have a snapshot we are raising the batches downloading process that will occurs
            await InterceptAsync(new HttpBatchesDownloadingArgs(_syncContext, StartTime.Value, serverBatchInfo, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // function used to download one part
            var dl = new Func<BatchPartInfo, Task>(async (bpi) =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);

                var serializer3 = SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3).ConfigureAwait(false);
                var step3 = HttpStep.GetMoreChanges;

                await InterceptAsync(new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

                // Raise get changes request
                ctx.ProgressPercentage = initialPctProgress + (bpi.Index + 1) * 0.2d / serverBatchInfo.BatchPartsInfo.Count;

                var response = await _httpRequestHandler.ProcessRequestAsync(
                HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, ScopeName,
                SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                if (SerializerFactory.Key != "json")
                {
                    var s = SerializerFactory.GetSerializer<HttpMessageSendChangesResponse>();
                    using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    var getMoreChanges = await s.DeserializeAsync(responseStream);

                    if (getMoreChanges is not null && getMoreChanges.Changes is not null && getMoreChanges.Changes.HasRows)
                    {
                        var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();

                        // Should have only one table
                        var table = getMoreChanges.Changes.Tables[0];
                        var schemaTable = DbSyncAdapter.CreateChangesTable(schema.Tables[table.TableName, table.SchemaName]);

                        var fullPath = Path.Combine(serverBatchInfo.GetDirectoryFullPath(), bpi.FileName);

                        // open the file and write table header
                        await localSerializer.OpenFileAsync(fullPath, schemaTable).ConfigureAwait(false);

                        foreach (var row in table.Rows)
                            await localSerializer.WriteRowToFileAsync(new SyncRow(schemaTable, row), schemaTable).ConfigureAwait(false);

                        // Close file
                        await localSerializer.CloseFileAsync(fullPath, schemaTable).ConfigureAwait(false);
                    }

                }
                else
                {
                    // Serialize
                    await SerializeAsync(response, bpi.FileName, serverBatchInfo.GetDirectoryFullPath(), this).ConfigureAwait(false);
                }

                // Raise response from server containing a batch changes 
                await InterceptAsync(new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);
            });

            // Parrallel download of all bpis (which will launch the delete directory on the server side)
            await serverBatchInfo.BatchPartsInfo.ForEachAsync(bpi => dl(bpi), MaxDownladingDegreeOfParallelism).ConfigureAwait(false);

            // Send order of end of download
            var lastBpi = serverBatchInfo.BatchPartsInfo.FirstOrDefault(bpi => bpi.IsLastBatch);

            if (lastBpi is not null)
            {
                var endOfDownloadChanges = new HttpMessageGetMoreChangesRequest(ctx, lastBpi.Index);

                var serializerEndOfDownloadChanges = SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializerEndOfDownloadChanges.SerializeAsync(endOfDownloadChanges).ConfigureAwait(false);

                await _httpRequestHandler.ProcessRequestAsync(
                    HttpClient, ServiceUri, binaryData3, HttpStep.SendEndDownloadChanges, ctx.SessionId, ScopeName,
                    SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);
            }

            // generate the new scope item
            CompleteTime = DateTime.UtcNow;

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            await InterceptAsync(new HttpBatchesDownloadedArgs(summaryResponseContent, summaryResponseContent.SyncContext, StartTime.Value, DateTime.UtcNow, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            return (summaryResponseContent.RemoteClientTimestamp, serverBatchInfo, summaryResponseContent.ConflictResolutionPolicy,
                    summaryResponseContent.ClientChangesApplied, summaryResponseContent.ServerChangesSelected);
        }


        public override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected DatabaseChangesSelected)>
          GetSnapshotAsync(SyncSet schema = null, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            // Get context or create a new one
            var ctx = GetContext();

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

            // Make a remote call to get Schema from remote provider
            if (schema is null)
            {
                var serverScopeInfo = await EnsureSchemaAsync(connection, transaction, cancellationToken, progress).ConfigureAwait(false);
                schema = serverScopeInfo.Schema;
                schema.EnsureSchema();
            }

            ctx.SyncStage = SyncStage.SnapshotApplying;

            // Generate a batch directory
            var batchDirectoryRoot = Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));
            var batchDirectoryFullPath = Path.Combine(batchDirectoryRoot, batchDirectoryName);

            if (!Directory.Exists(batchDirectoryFullPath))
                Directory.CreateDirectory(batchDirectoryFullPath);

            // Create the BatchInfo serialized (forced because in a snapshot call, so we are obviously serialized on disk)
            var serverBatchInfo = new BatchInfo(schema, batchDirectoryRoot, batchDirectoryName);

            // Firstly, get the snapshot summary
            var changesToSend = new HttpMessageSendChangesRequest(ctx, null);
            var serializer = SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);
            var step = HttpStep.GetSummary;

            var response0 = await _httpRequestHandler.ProcessRequestAsync(
              HttpClient, ServiceUri, binaryData, step, ctx.SessionId, ScopeName,
              SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageSummaryResponse summaryResponseContent = null;

            using (var streamResponse = await response0.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();

                if (streamResponse.CanRead)
                {
                    summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);

                    serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo?.RowsCount ?? 0;

                    if (summaryResponseContent.BatchInfo?.BatchPartsInfo is not null)
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
            await InterceptAsync(new HttpBatchesDownloadingArgs(_syncContext, StartTime.Value, serverBatchInfo, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            await serverBatchInfo.BatchPartsInfo.ForEachAsync(async bpi =>
            {
                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);
                var serializer3 = SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3);
                var step3 = HttpStep.GetMoreChanges;

                await InterceptAsync(new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

                var response = await _httpRequestHandler.ProcessRequestAsync(
                  HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, ScopeName,
                  SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                if (SerializerFactory.Key != "json")
                {
                    var s = SerializerFactory.GetSerializer<HttpMessageSendChangesResponse>();
                    using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    var getMoreChanges = await s.DeserializeAsync(responseStream);

                    if (getMoreChanges is not null && getMoreChanges.Changes is not null && getMoreChanges.Changes.HasRows)
                    {
                        var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();

                        // Should have only one table
                        var table = getMoreChanges.Changes.Tables[0];
                        var schemaTable = DbSyncAdapter.CreateChangesTable(schema.Tables[table.TableName, table.SchemaName]);

                        var fullPath = Path.Combine(batchDirectoryFullPath, bpi.FileName);

                        // open the file and write table header
                        await localSerializer.OpenFileAsync(fullPath, schemaTable).ConfigureAwait(false);

                        foreach (var row in table.Rows)
                            await localSerializer.WriteRowToFileAsync(new SyncRow(schemaTable, row), schemaTable).ConfigureAwait(false);

                        // Close file
                        await localSerializer.CloseFileAsync(fullPath, schemaTable).ConfigureAwait(false);
                    }

                }
                else
                {
                    // Serialize
                    await SerializeAsync(response, bpi.FileName, batchDirectoryFullPath, this).ConfigureAwait(false);
                }


                // Raise response from server containing a batch changes 
                await InterceptAsync(new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);
            });

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            await InterceptAsync(new HttpBatchesDownloadedArgs(summaryResponseContent, summaryResponseContent.SyncContext, StartTime.Value, DateTime.UtcNow, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

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
        public override Task<bool> NeedsToUpgradeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
                        => throw new NotImplementedException();

        /// <summary>
        /// Not Allowed from WebClientOrchestrator
        /// </summary>
        public override Task<bool> UpgradeAsync(DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
                        => throw new NotImplementedException();


        /// <summary>
        /// We can't get changes from server, from a web client orchestrator
        /// </summary>
        public override async Task<(long RemoteClientTimestamp, BatchInfo ServerBatchInfo, DatabaseChangesSelected ServerChangesSelected)>
                                GetChangesAsync(ScopeInfo clientScope, DbConnection connection = default, DbTransaction transaction = default, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {


            SyncSet schema;
            // Get context or create a new one
            var ctx = GetContext();

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

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

            await InterceptAsync(new HttpSendingClientChangesRequestArgs(changesToSend, 0, 0, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // serialize message
            var serializer = SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);

            var response = await _httpRequestHandler.ProcessRequestAsync
                (HttpClient, ServiceUri, binaryData, HttpStep.SendChangesInProgress, ctx.SessionId, clientScope.Name,
                 SerializerFactory, Converter, Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);



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
            var serverBatchInfo = new BatchInfo(schema);

            HttpMessageSummaryResponse summaryResponseContent = null;

            // Deserialize response incoming from server
            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = SerializerFactory.GetSerializer<HttpMessageSummaryResponse>();
                summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);
            }

            serverBatchInfo.RowsCount = summaryResponseContent.BatchInfo.RowsCount;
            serverBatchInfo.Timestamp = summaryResponseContent.RemoteClientTimestamp;

            if (summaryResponseContent.BatchInfo.BatchPartsInfo is not null)
                foreach (var bpi in summaryResponseContent.BatchInfo.BatchPartsInfo)
                    serverBatchInfo.BatchPartsInfo.Add(bpi);

            //-----------------------
            // In Batch Mode
            //-----------------------
            // From here, we need to serialize everything on disk

            // Generate the batch directory
            var batchDirectoryRoot = Options.BatchDirectory;
            var batchDirectoryName = string.Concat(DateTime.UtcNow.ToString("yyyy_MM_dd_ss"), Path.GetRandomFileName().Replace(".", ""));

            serverBatchInfo.DirectoryRoot = batchDirectoryRoot;
            serverBatchInfo.DirectoryName = batchDirectoryName;

            if (!Directory.Exists(serverBatchInfo.GetDirectoryFullPath()))
                Directory.CreateDirectory(serverBatchInfo.GetDirectoryFullPath());


            // hook to get the last batch part info at the end
            var bpis = serverBatchInfo.BatchPartsInfo.Where(bpi => !bpi.IsLastBatch);
            var lstbpi = serverBatchInfo.BatchPartsInfo.First(bpi => bpi.IsLastBatch);

            // function used to download one part
            var dl = new Func<BatchPartInfo, Task>(async (bpi) =>
            {
                var changesToSend3 = new HttpMessageGetMoreChangesRequest(ctx, bpi.Index);
                var serializer3 = SerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>();
                var binaryData3 = await serializer3.SerializeAsync(changesToSend3).ConfigureAwait(false);
                var step3 = HttpStep.GetMoreChanges;

                await InterceptAsync(new HttpGettingServerChangesRequestArgs(bpi.Index, serverBatchInfo.BatchPartsInfo.Count, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

                // Raise get changes request
                ctx.ProgressPercentage = initialPctProgress + (bpi.Index + 1) * 0.2d / serverBatchInfo.BatchPartsInfo.Count;

                var response = await _httpRequestHandler.ProcessRequestAsync(
                HttpClient, ServiceUri, binaryData3, step3, ctx.SessionId, ScopeName,
                SerializerFactory, Converter, 0, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

                // Serialize
                await SerializeAsync(response, bpi.FileName, serverBatchInfo.GetDirectoryFullPath(), this).ConfigureAwait(false);

                // Raise response from server containing a batch changes 
                await InterceptAsync(new HttpGettingServerChangesResponseArgs(serverBatchInfo, bpi.Index, bpi.RowsCount, summaryResponseContent.SyncContext, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);
            });

            // Parrallel download of all bpis except the last one (which will launch the delete directory on the server side)
            await bpis.ForEachAsync(bpi => dl(bpi), MaxDownladingDegreeOfParallelism).ConfigureAwait(false);

            // Download last batch part that will launch the server deletion of the tmp dir
            await dl(lstbpi).ConfigureAwait(false);

            // generate the new scope item
            CompleteTime = DateTime.UtcNow;

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
            var ctx = GetContext();

            if (!StartTime.HasValue)
                StartTime = DateTime.UtcNow;

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

            var serializer = SerializerFactory.GetSerializer<HttpMessageSendChangesRequest>();
            var binaryData = await serializer.SerializeAsync(changesToSend);

            // Raise progress for sending request and waiting server response
            await InterceptAsync(new HttpGettingServerChangesRequestArgs(0, 0, ctx, GetServiceHost()), progress, cancellationToken).ConfigureAwait(false);

            // response
            var response = await _httpRequestHandler.ProcessRequestAsync
                    (HttpClient, ServiceUri, binaryData, HttpStep.GetEstimatedChangesCount, ctx.SessionId, clientScope.Name,
                     SerializerFactory, Converter, Options.BatchSize, SyncPolicy, cancellationToken, progress).ConfigureAwait(false);

            HttpMessageSendChangesResponse summaryResponseContent = null;

            using (var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var responseSerializer = SerializerFactory.GetSerializer<HttpMessageSendChangesResponse>();

                if (streamResponse.CanRead)
                    summaryResponseContent = await responseSerializer.DeserializeAsync(streamResponse);

            }

            if (summaryResponseContent is null)
                throw new Exception("Summary can't be null");

            // generate the new scope item
            CompleteTime = DateTime.UtcNow;

            // Reaffect context
            SetContext(summaryResponseContent.SyncContext);

            return (summaryResponseContent.RemoteClientTimestamp, summaryResponseContent.ServerChangesSelected);
        }



        public void BeforeSerializeRows(ContainerTable table, SyncTable schemaTable, IConverter converter)
        {
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                    converter.BeforeSerialize(row, schemaTable);
            }
        }

        public void AfterDeserializedRows(ContainerTable table, SyncTable schemaTable, IConverter converter)
        {
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                    converter.AfterDeserialized(row, schemaTable);

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
                SyncContext syncContext = GetContext();
                IProgress<ProgressArgs> progressArgs = arg as IProgress<ProgressArgs>;
                await InterceptAsync(new HttpSyncPolicyArgs(syncContext, 10, cpt, ts), default).ConfigureAwait(false);
            });


            return policy;

        }
        private static async Task SerializeAsync(HttpResponseMessage response, string fileName, string directoryFullPath, BaseOrchestrator orchestrator = null)
        {
            var fullPath = Path.Combine(directoryFullPath, fileName);
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
