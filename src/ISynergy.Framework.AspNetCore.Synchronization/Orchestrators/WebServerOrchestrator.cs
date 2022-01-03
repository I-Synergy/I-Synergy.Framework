using ISynergy.Framework.AspNetCore.Synchronization.Arguments;
using ISynergy.Framework.AspNetCore.Synchronization.Cache;
using ISynergy.Framework.AspNetCore.Synchronization.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Synchronization.Client.Enumerations;
using ISynergy.Framework.Synchronization.Client.Exceptions;
using ISynergy.Framework.Synchronization.Client.Messages;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Abstractions;
using ISynergy.Framework.Synchronization.Core.Adapters;
using ISynergy.Framework.Synchronization.Core.Algorithms;
using ISynergy.Framework.Synchronization.Core.Batch;
using ISynergy.Framework.Synchronization.Core.Builders;
using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Messages;
using ISynergy.Framework.Synchronization.Core.Models;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Serialization;
using ISynergy.Framework.Synchronization.Core.Set;
using ISynergy.Framework.Synchronization.Core.Setup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Orchestrators
{
    public class WebServerOrchestrator : RemoteOrchestrator
    {
        /// <summary>
        /// Gets or sets converters.
        /// </summary>
        public Collection<IConverter> Converters { get; set; }

        /// <summary>
        /// Gets or sets serializer factories.
        /// </summary>
        public Collection<ISerializerFactory> SerializerFactories { get; set; }

        /// <summary>
        /// Gets or Sets the Client Converter
        /// </summary>
        public IConverter ClientConverter { get; private set; }

        /// <summary>
        /// Default ctor. Using default options and schema
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        /// <param name="setup"></param>
        /// <param name="scopeName"></param>
        public WebServerOrchestrator(
            IVersionService versionService, 
            IProvider provider, 
            SyncOptions options, 
            SyncSetup setup, 
            string scopeName = SyncOptions.DefaultScopeName)
            : base(versionService, provider, options, setup, scopeName)
        {
            Converters = new Collection<IConverter>();

            SerializerFactories = new Collection<ISerializerFactory>
            {
                SerializersCollection.JsonSerializerFactory
            };
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="versionService"></param>
        /// <param name="provider"></param>
        /// <param name="tables"></param>
        /// <param name="scopeName"></param>
        public WebServerOrchestrator(
            IVersionService versionService, 
            IProvider provider, 
            string[] tables, 
            string scopeName = SyncOptions.DefaultScopeName)
            : this(versionService, provider, new SyncOptions(), new SyncSetup(tables), scopeName)
        {
        }

        /// <summary>
        /// Call this method to handle requests on the server, sent by the client
        /// </summary>
        public Task HandleRequestAsync(HttpContext context, CancellationToken token = default, IProgress<ProgressArgs> progress = null) =>
            HandleRequestAsync(context, null, token, progress);

        /// <summary>
        /// Gets list of file metadata
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Task<List<FileInfoMetadata>> GetFileMetadataAsync(string path)
        {
            var filePath = Path.GetFullPath(path);
            var rootFolder = new DirectoryInfo(filePath);
            var files = rootFolder.GetFiles("*.*", SearchOption.AllDirectories);
            return Task.FromResult(files.Select(s => new FileInfoMetadata(rootFolder.FullName, s)).ToList());
        }

        /// <summary>
        /// Call this method to handle requests on the server, sent by the client
        /// </summary>
        public async Task HandleRequestAsync(HttpContext httpContext, Action<RemoteOrchestrator> action, CancellationToken cancellationToken, IProgress<ProgressArgs> progress)
        {
            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;
            var serAndsizeString = string.Empty;
            var cliConverterKey = string.Empty;

            if (TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-serialization-format", out var vs))
                serAndsizeString = vs.ToLowerInvariant();

            if (TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-converter", out var cs))
                cliConverterKey = cs.ToLowerInvariant();

            if (!TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-session-id", out var sessionId))
                throw new HttpHeaderMissingException("isynergy-sync-session-id");

            if (!TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-scope-name", out var scopeName))
                throw new HttpHeaderMissingException("isynergy-sync-scope-name");

            if (!TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-step", out string iStep))
                throw new HttpHeaderMissingException("isynergy-sync-step");

            var step = (HttpStep)Convert.ToInt32(iStep);
            var readableStream = new MemoryStream();

            try
            {
                // Copty stream to a readable and seekable stream
                // HttpRequest.Body is a HttpRequestStream that is readable but can't be Seek
                await httpRequest.Body.CopyToAsync(readableStream);
                httpRequest.Body.Close();
                httpRequest.Body.Dispose();

                // if Hash is present in header, check hash
                if (TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-hash", out string hashStringRequest))
                    HashAlgorithm.SHA256.EnsureHash(readableStream, hashStringRequest);
                else
                    readableStream.Seek(0, SeekOrigin.Begin);

                // load session
                await httpContext.Session.LoadAsync(cancellationToken);

                // Get schema and clients batch infos / summaries, from session
                var schema = httpContext.Session.Get<SyncSet>(scopeName);
                var sessionCache = httpContext.Session.Get<SessionCache>(sessionId);

                // HttpStep.EnsureSchema is the first call from client when client is new
                // HttpStep.EnsureScopes is the first call from client when client is not new
                // This is the only moment where we are initializing the sessionCache and store it in session
                if (sessionCache is null && (step == HttpStep.EnsureSchema || step == HttpStep.EnsureScopes))
                {
                    sessionCache = new SessionCache();
                    httpContext.Session.Set(sessionId, sessionCache);
                    httpContext.Session.SetString("session_id", sessionId);
                }

                // if sessionCache is still null, then we are in a step where it should not be null.
                // Probably because of a weird server restart or something...
                if (sessionCache is null)
                    throw new HttpSessionLostException();

                // check session id
                var tempSessionId = httpContext.Session.GetString("session_id");

                if (string.IsNullOrEmpty(tempSessionId) || tempSessionId != sessionId)
                    throw new HttpSessionLostException();

                // Check if sanitized schema is still there
                if (sessionCache.ClientBatchInfo is not null
                    && sessionCache.ClientBatchInfo.SanitizedSchema is not null && sessionCache.ClientBatchInfo.SanitizedSchema.Tables.Count == 0
                    && schema is not null && schema.Tables.Count > 0)
                    foreach (var table in schema.Tables)
                        DbSyncAdapter.CreateChangesTable(schema.Tables[table.TableName, table.SchemaName], sessionCache.ClientBatchInfo.SanitizedSchema);

                // action from user if available
                action?.Invoke(this);

                // Get the serializer and batchsize
                (var clientBatchSize, var clientSerializerFactory) = GetClientSerializer(serAndsizeString);

                // Get converter used by client
                // Can be null
                var clientConverter = GetClientConverter(cliConverterKey);
                ClientConverter = clientConverter;

                byte[] binaryData = null;
                switch (step)
                {
                    case HttpStep.EnsureScopes:
                        var m1 = await clientSerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m1.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s1 = await EnsureScopesAsync(httpContext, m1, sessionCache, cancellationToken, progress).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageEnsureScopesResponse>().SerializeAsync(s1);
                        break;
                    case HttpStep.EnsureSchema:
                        var m11 = await clientSerializerFactory.GetSerializer<HttpMessageEnsureScopesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m11.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s11 = await EnsureSchemaAsync(httpContext, m11, sessionCache, cancellationToken, progress).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageEnsureSchemaResponse>().SerializeAsync(s11);
                        break;

                    // version >= 0.8    
                    case HttpStep.SendChangesInProgress:
                        var m22 = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m22.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        await InterceptAsync(new HttpGettingClientChangesArgs(m22, httpContext.Request.Host.Host, sessionCache), progress, cancellationToken).ConfigureAwait(false);
                        var s22 = await ApplyThenGetChangesAsync2(httpContext, m22, sessionCache, clientBatchSize, cancellationToken, progress).ConfigureAwait(false);
                        //await InterceptAsync(new HttpSendingServerChangesArgs(s22.HttpMessageSendChangesResponse, context.Request.Host.Host, sessionCache, false), cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSummaryResponse>().SerializeAsync(s22);
                        break;

                    case HttpStep.GetChanges:
                        var m3 = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m3.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        await InterceptAsync(new HttpGettingClientChangesArgs(m3, httpContext.Request.Host.Host, sessionCache), progress, cancellationToken).ConfigureAwait(false);
                        var s3 = await GetChangesAsync(httpContext, m3, sessionCache, clientBatchSize, cancellationToken, progress);
                        await InterceptAsync(new HttpSendingServerChangesArgs(s3, httpContext.Request.Host.Host, sessionCache, false), progress, cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesResponse>().SerializeAsync(s3);
                        break;

                    case HttpStep.GetMoreChanges:
                        var m4 = await clientSerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m4.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s4 = await GetMoreChangesAsync(httpContext, m4, sessionCache, cancellationToken, progress);
                        await InterceptAsync(new HttpSendingServerChangesArgs(s4, httpContext.Request.Host.Host, sessionCache, false), progress, cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesResponse>().SerializeAsync(s4);
                        break;

                    case HttpStep.GetSnapshot:
                        var m5 = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m5.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s5 = await GetSnapshotAsync(httpContext, m5, sessionCache, cancellationToken, progress);
                        await InterceptAsync(new HttpSendingServerChangesArgs(s5, httpContext.Request.Host.Host, sessionCache, true), progress, cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesResponse>().SerializeAsync(s5);
                        break;

                    // version >= 0.8    
                    case HttpStep.GetSummary:
                        var m55 = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m55.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s55 = await GetSnapshotSummaryAsync(httpContext, m55, sessionCache, cancellationToken, progress);
                        //await InterceptAsync(new HttpSendingServerChangesArgs(s5.HttpMessageSendChangesResponse, context.Request.Host.Host, sessionCache, true), cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSummaryResponse>().SerializeAsync(s55);

                        break;
                    case HttpStep.SendEndDownloadChanges:
                        var m56 = await clientSerializerFactory.GetSerializer<HttpMessageGetMoreChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m56.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s56 = await SendEndDownloadChangesAsync(httpContext, m56, sessionCache, cancellationToken, progress);
                        await InterceptAsync(new HttpSendingServerChangesArgs(s56, httpContext.Request.Host.Host, sessionCache, false), progress, cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesResponse>().SerializeAsync(s56);
                        break;

                    case HttpStep.GetEstimatedChangesCount:
                        var m6 = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesRequest>().DeserializeAsync(readableStream);
                        await InterceptAsync(new HttpGettingRequestArgs(httpContext, m6.SyncContext, sessionCache, step), progress, cancellationToken).ConfigureAwait(false);
                        var s6 = await GetEstimatedChangesCountAsync(httpContext, m6, cancellationToken, progress);
                        await InterceptAsync(new HttpSendingServerChangesArgs(s6, httpContext.Request.Host.Host, sessionCache, false), progress, cancellationToken).ConfigureAwait(false);
                        binaryData = await clientSerializerFactory.GetSerializer<HttpMessageSendChangesResponse>().SerializeAsync(s6);
                        break;
                }

                httpContext.Session.Set(scopeName, schema);
                httpContext.Session.Set(sessionId, sessionCache);
                await httpContext.Session.CommitAsync(cancellationToken);

                // Adding the serialization format used and session id
                httpResponse.Headers.Add("isynergy-sync-session-id", sessionId.ToString());
                httpResponse.Headers.Add("isynergy-sync-serialization-format", clientSerializerFactory.Key);

                // calculate hash
                var hash = HashAlgorithm.SHA256.Create(binaryData);
                var hashString = Convert.ToBase64String(hash);
                // Add hash to header
                httpResponse.Headers.Add("isynergy-sync-hash", hashString);

                // data to send back, as the response
                byte[] data = EnsureCompression(httpRequest, httpResponse, binaryData);

                // save session
                await httpContext.Session.CommitAsync(cancellationToken);

                await InterceptAsync(new HttpSendingResponseArgs(httpContext, GetContext(), sessionCache, data, step), progress, cancellationToken).ConfigureAwait(false);

                await httpResponse.Body.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await WriteExceptionAsync(httpRequest, httpResponse, ex);
            }
            finally
            {
                readableStream.Flush();
                readableStream.Close();
                readableStream.Dispose();
            }
        }

        /// <summary>
        /// Ensure we have a Compression setting or not
        /// </summary>
        private byte[] EnsureCompression(HttpRequest httpRequest, HttpResponse httpResponse, byte[] binaryData)
        {
            string encoding = httpRequest.Headers["Accept-Encoding"];

            // Compress data if client accept Gzip / Deflate
            if (!string.IsNullOrEmpty(encoding) && (encoding.Contains("gzip") || encoding.Contains("deflate")))
            {
                if (!httpResponse.Headers.ContainsKey("Content-Encoding"))
                    httpResponse.Headers.Add("Content-Encoding", "gzip");

                using var writeSteam = new MemoryStream();

                using (var compress = new GZipStream(writeSteam, CompressionMode.Compress))
                {
                    compress.Write(binaryData, 0, binaryData.Length);
                    compress.Flush();
                }

                var b = writeSteam.ToArray();
                writeSteam.Flush();
                return b;
            }

            return binaryData;
        }

        private (int clientBatchSize, ISerializerFactory clientSerializer) GetClientSerializer(string serAndsizeString)
        {
            try
            {
                if (string.IsNullOrEmpty(serAndsizeString))
                    throw new Exception("Serializer header is null, coming from http header");

                var serAndsize = JsonConvert.DeserializeAnonymousType(serAndsizeString, new { f = "", s = 0 });

                var clientBatchSize = serAndsize.s;

                var clientSerializerFactory = SerializerFactories.FirstOrDefault(sf => sf.Key == serAndsize.f);
                if (clientSerializerFactory is null) clientSerializerFactory = SerializersCollection.JsonSerializerFactory;

                return (clientBatchSize, clientSerializerFactory);
            }
            catch
            {
                throw new Exception("Serializer header is incorrect, coming from http header");
                //throw new HttpSerializerNotConfiguredException(WebServerOptions.Serializers.Select(sf => sf.Key));
            }
        }

        private IConverter GetClientConverter(string cliConverterKey)
        {
            try
            {
                if (string.IsNullOrEmpty(cliConverterKey))
                    return null;

                var clientConverter = Converters.First(c => c.Key.ToLowerInvariant() == cliConverterKey);

                return clientConverter;
            }
            catch
            {
                throw new HttpConverterNotConfiguredException(Converters.Select(sf => sf.Key));
            }
        }


        /// <summary>
        /// Get Scope Name sent by the client
        /// </summary>
        public static string GetScopeName(HttpContext httpContext) => TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-scope-name", out var val) ? val : null;

        /// <summary>
        /// Get the current client session id
        /// </summary>
        public static string GetClientSessionId(HttpContext httpContext) => TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-session-id", out var val) ? val : null;

        /// <summary>
        /// Get the current Step
        /// </summary>
        public static HttpStep GetCurrentStep(HttpContext httpContext) => TryGetHeaderValue(httpContext.Request.Headers, "isynergy-sync-step", out var val) ? (HttpStep)Convert.ToInt32(val) : HttpStep.None;

        public static bool TryGetHeaderValue(IHeaderDictionary n, string key, out string header)
        {
            if (n.TryGetValue(key, out var vs))
            {
                header = vs[0];
                return true;
            }

            header = null;
            return false;
        }

        internal async Task<HttpMessageEnsureScopesResponse> EnsureScopesAsync(
            HttpContext httpContext, 
            HttpMessageEnsureScopesRequest httpMessage, 
            SessionCache sessionCache,
            CancellationToken cancellationToken, 
            IProgress<ProgressArgs> progress = null)
        {
            Argument.IsNotNull(httpContext);
            Argument.IsNotNull(Setup);

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            // Get schema
            var serverScopeInfo = await GetServerScopeAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

            // Create http response
            var httpResponse = new HttpMessageEnsureScopesResponse(ctx, serverScopeInfo);

            return httpResponse;
        }


        internal async Task<HttpMessageEnsureSchemaResponse> EnsureSchemaAsync(HttpContext httpContext, HttpMessageEnsureScopesRequest httpMessage, SessionCache sessionCache,
            CancellationToken cancellationToken, IProgress<ProgressArgs> progress = null)
        {
            Argument.IsNotNull(httpContext);
            Argument.IsNotNull(Setup);

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            // Get schema
            var serverScopeInfo = await base.EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

            var schema = serverScopeInfo.Schema;
            schema.EnsureSchema();
            httpContext.Session.Set(httpMessage.SyncContext.ScopeName, schema);

            var httpResponse = new HttpMessageEnsureSchemaResponse(ctx, serverScopeInfo);

            return httpResponse;


        }

        internal async Task<HttpMessageSendChangesResponse> GetChangesAsync(HttpContext httpContext, HttpMessageSendChangesRequest httpMessage, SessionCache sessionCache,
                        int clientBatchSize, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {

            // Overriding batch size options value, coming from client
            // having changes from server in batch size or not is decided by the client.
            // Basically this options is not used on the server, since it's always overriden by the client
            Options.BatchSize = clientBatchSize;

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            var changes = await base.GetChangesAsync(httpMessage.Scope, default, default, cancellationToken, progress);

            // no changes applied to server
            var clientChangesApplied = new DatabaseChangesApplied();

            // Save the server batch info object to cache if not working in memory
            sessionCache.RemoteClientTimestamp = changes.RemoteClientTimestamp;
            sessionCache.ServerBatchInfo = changes.ServerBatchInfo;
            sessionCache.ServerChangesSelected = changes.ServerChangesSelected;
            sessionCache.ClientChangesApplied = clientChangesApplied;

            // Get the firt response to send back to client
            return await GetChangesResponseAsync(httpContext, ctx, changes.RemoteClientTimestamp, changes.ServerBatchInfo, clientChangesApplied, changes.ServerChangesSelected, 0);
        }

        internal async Task<HttpMessageSendChangesResponse> GetEstimatedChangesCountAsync(HttpContext httpContext, HttpMessageSendChangesRequest httpMessage,
                        CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            var changes = await base.GetEstimatedChangesCountAsync(httpMessage.Scope, default, default, cancellationToken, progress);

            var changesResponse = new HttpMessageSendChangesResponse(_syncContext)
            {
                ServerChangesSelected = changes.ServerChangesSelected,
                ClientChangesApplied = new DatabaseChangesApplied(),
                ServerStep = HttpStep.GetMoreChanges,
                ConflictResolutionPolicy = Options.ConflictResolutionPolicy,
                IsLastBatch = true,
                RemoteClientTimestamp = changes.RemoteClientTimestamp
            };

            return changesResponse;
        }

        internal async Task<HttpMessageSummaryResponse> GetSnapshotSummaryAsync(HttpContext httpContext, HttpMessageSendChangesRequest httpMessage, SessionCache sessionCache,
                        CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Check schema.
            // If client has stored the schema, the EnsureScope will not be called on server.
            var schema = await EnsureSchemaFromSessionAsync(httpContext, httpMessage.SyncContext.ScopeName, progress, cancellationToken).ConfigureAwait(false);

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            // get changes
            var snap = await GetSnapshotAsync(schema, default, default, cancellationToken, progress).ConfigureAwait(false);

            var summaryResponse = new HttpMessageSummaryResponse(ctx)
            {
                BatchInfo = snap.ServerBatchInfo,
                RemoteClientTimestamp = snap.RemoteClientTimestamp,
                ClientChangesApplied = new DatabaseChangesApplied(),
                ServerChangesSelected = snap.DatabaseChangesSelected,
                ConflictResolutionPolicy = Options.ConflictResolutionPolicy,
                Step = HttpStep.GetSummary,
            };

            // Save the server batch info object to cache
            sessionCache.RemoteClientTimestamp = snap.RemoteClientTimestamp;
            sessionCache.ServerBatchInfo = snap.ServerBatchInfo;
            sessionCache.ServerChangesSelected = snap.DatabaseChangesSelected;

            return summaryResponse;
        }

        private async Task<SyncSet> EnsureSchemaFromSessionAsync(HttpContext httpContext, string scopeName, IProgress<ProgressArgs> progress, CancellationToken cancellationToken)
        {
            var schema = httpContext.Session.Get<SyncSet>(scopeName);

            if (schema is null || !schema.HasTables || !schema.HasColumns)
            {
                var serverScopeInfo = await base.EnsureSchemaAsync(default, default, cancellationToken, progress).ConfigureAwait(false);

                schema = serverScopeInfo.Schema;
                schema.EnsureSchema();
                httpContext.Session.Set(scopeName, schema);
            }

            return schema;
        }

        internal async Task<HttpMessageSendChangesResponse> GetSnapshotAsync(HttpContext httpContext, HttpMessageSendChangesRequest httpMessage, SessionCache sessionCache,
                            CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Check schema.
            var schema = await EnsureSchemaFromSessionAsync(httpContext, httpMessage.SyncContext.ScopeName, progress, cancellationToken).ConfigureAwait(false);

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            // get changes
            var snap = await GetSnapshotAsync(schema, default, default, cancellationToken, progress).ConfigureAwait(false);

            // Save the server batch info object to cache
            sessionCache.RemoteClientTimestamp = snap.RemoteClientTimestamp;
            sessionCache.ServerBatchInfo = snap.ServerBatchInfo;
            sessionCache.ServerChangesSelected = snap.DatabaseChangesSelected;
            //httpContext.Session.Set(sessionId, sessionCache);

            // if no snapshot, return empty response
            if (snap.ServerBatchInfo is null)
            {
                var changesResponse = new HttpMessageSendChangesResponse(ctx)
                {
                    ServerStep = HttpStep.GetSnapshot,
                    BatchIndex = 0,
                    BatchCount = 0,
                    IsLastBatch = true,
                    RemoteClientTimestamp = 0,
                    Changes = null
                };
                return changesResponse;
            }

            sessionCache.RemoteClientTimestamp = snap.RemoteClientTimestamp;
            sessionCache.ServerBatchInfo = snap.ServerBatchInfo;

            // Get the firt response to send back to client
            return await GetChangesResponseAsync(httpContext, ctx, snap.RemoteClientTimestamp, snap.ServerBatchInfo, null, snap.DatabaseChangesSelected, 0);
        }

        /// <summary>
        /// Get changes from 
        /// </summary>
        internal async Task<HttpMessageSummaryResponse> ApplyThenGetChangesAsync2(HttpContext httpContext, HttpMessageSendChangesRequest httpMessage, SessionCache sessionCache,
                        int clientBatchSize, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            // Overriding batch size options value, coming from client
            // having changes from server in batch size or not is decided by the client.
            // Basically this options is not used on the server, since it's always overriden by the client
            Options.BatchSize = clientBatchSize;

            // Get context from request message
            var ctx = httpMessage.SyncContext;

            // Set the context coming from the client
            SetContext(ctx);

            // Check schema.
            var schema = await EnsureSchemaFromSessionAsync(httpContext, httpMessage.SyncContext.ScopeName, progress, cancellationToken).ConfigureAwait(false);

            // ------------------------------------------------------------
            // FIRST STEP : receive client changes
            // ------------------------------------------------------------

            // We are receiving changes from client
            // BatchInfo containing all BatchPartInfo objects
            // Retrieve batchinfo instance if exists
            // Get batch info from session cache if exists, otherwise create it
            if (sessionCache.ClientBatchInfo is null)
            {
                sessionCache.ClientBatchInfo = schema.ToBatchInfo(Options.BatchDirectory);
                sessionCache.ClientBatchInfo.TryRemoveDirectory();
                sessionCache.ClientBatchInfo.CreateDirectory();
            }

            if (httpMessage.Changes is not null && httpMessage.Changes.HasRows)
            {
                // we have only one table here
                var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();
                var containerTable = httpMessage.Changes.Tables[0];
                var schemaTable = DbSyncAdapter.CreateChangesTable(schema.Tables[containerTable.TableName, containerTable.SchemaName]);
                var tableName = ParserName.Parse(new SyncTable(containerTable.TableName, containerTable.SchemaName)).Unquoted().Schema().Normalized().ToString();
                var fileName = BatchInfo.GenerateNewFileName(httpMessage.BatchIndex.ToString(), tableName, localSerializer.Extension);
                var fullPath = Path.Combine(sessionCache.ClientBatchInfo.GetDirectoryFullPath(), fileName);

                // If client has made a conversion on each line, apply the reverse side of it
                if (ClientConverter is not null)
                    AfterDeserializedRows(containerTable, schemaTable, ClientConverter);

                // open the file and write table header
                await localSerializer.OpenFileAsync(fullPath, schemaTable).ConfigureAwait(false);

                foreach (var row in containerTable.Rows)
                    await localSerializer.WriteRowToFileAsync(new SyncRow(schemaTable, row), schemaTable).ConfigureAwait(false);

                // Close file
                await localSerializer.CloseFileAsync(fullPath, schemaTable).ConfigureAwait(false);

                // Create the info on the batch part
                BatchPartTableInfo tableInfo = new BatchPartTableInfo
                {
                    TableName = containerTable.TableName,
                    SchemaName = containerTable.SchemaName,
                    RowsCount = containerTable.Rows.Count

                };
                var bpi = new BatchPartInfo { FileName = fileName };
                bpi.Tables = new BatchPartTableInfo[] { tableInfo };
                bpi.RowsCount = tableInfo.RowsCount;
                bpi.IsLastBatch = httpMessage.IsLastBatch;
                bpi.Index = httpMessage.BatchIndex;
                sessionCache.ClientBatchInfo.RowsCount += bpi.RowsCount;
                sessionCache.ClientBatchInfo.BatchPartsInfo.Add(bpi);


            }

            // Clear the httpMessage set
            if (httpMessage.Changes is not null)
                httpMessage.Changes.Clear();

            // Until we don't have received all the batches, wait for more
            if (!httpMessage.IsLastBatch)
                return new HttpMessageSummaryResponse(ctx) { Step = HttpStep.SendChangesInProgress };

            // ------------------------------------------------------------
            // SECOND STEP : apply then return server changes
            // ------------------------------------------------------------

            // get changes
            var (remoteClientTimestamp, serverBatchInfo, _, clientChangesApplied, serverChangesSelected) =
                   await base.ApplyThenGetChangesAsync(httpMessage.Scope, sessionCache.ClientBatchInfo, default, default, cancellationToken, progress).ConfigureAwait(false);

            // Set session cache infos
            sessionCache.RemoteClientTimestamp = remoteClientTimestamp;
            sessionCache.ServerBatchInfo = serverBatchInfo;
            sessionCache.ServerChangesSelected = serverChangesSelected;
            sessionCache.ClientChangesApplied = clientChangesApplied;

            // delete the folder (not the BatchPartInfo, because we have a reference on it)
            var cleanFolder = Options.CleanFolder;

            if (cleanFolder)
                cleanFolder = await InternalCanCleanFolderAsync(ctx, sessionCache.ClientBatchInfo, default).ConfigureAwait(false);

            if (cleanFolder)
                sessionCache.ClientBatchInfo.Clear(true);

            // we do not need client batch info now
            sessionCache.ClientBatchInfo = null;

            // Retro compatiblité to version < 0.9.3
            if (serverBatchInfo.BatchPartsInfo is null)
                serverBatchInfo.BatchPartsInfo = new List<BatchPartInfo>();


            var summaryResponse = new HttpMessageSummaryResponse(ctx)
            {
                BatchInfo = serverBatchInfo,
                Step = HttpStep.GetSummary,
                RemoteClientTimestamp = remoteClientTimestamp,
                ClientChangesApplied = clientChangesApplied,
                ServerChangesSelected = serverChangesSelected,
                ConflictResolutionPolicy = Options.ConflictResolutionPolicy,
            };


            // Compatibility with last versions where InMemory is set
            if (clientBatchSize <= 0)
            {
                var containerSet = new ContainerSet();
                foreach (var table in serverBatchInfo.SanitizedSchema.Tables)
                {
                    var containerTable = new ContainerTable(table);
                    foreach (var part in serverBatchInfo.GetBatchPartsInfo(table))
                    {
                        var paths = serverBatchInfo.GetBatchPartInfoPath(part);
                        var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();
                        foreach (var syncRow in localSerializer.ReadRowsFromFile(paths.FullPath, table))
                        {
                            containerTable.Rows.Add(syncRow.ToArray());
                        }
                    }
                    if (containerTable.Rows.Count > 0)
                        containerSet.Tables.Add(containerTable);
                }

                summaryResponse.Changes = containerSet;
                summaryResponse.BatchInfo.BatchPartsInfo.Clear();
                summaryResponse.BatchInfo.BatchPartsInfo = null;

            }


            // Get the firt response to send back to client
            return summaryResponse;

        }

        /// <summary>
        /// This method is only used when batch mode is enabled on server and we need to send back mor BatchPartInfo 
        /// </summary>
        internal Task<HttpMessageSendChangesResponse> GetMoreChangesAsync(HttpContext httpContext, HttpMessageGetMoreChangesRequest httpMessage,
            SessionCache sessionCache, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            return GetChangesResponseAsync(httpContext, httpMessage.SyncContext, sessionCache.RemoteClientTimestamp,
                sessionCache.ServerBatchInfo, sessionCache.ClientChangesApplied,
                sessionCache.ServerChangesSelected, httpMessage.BatchIndexRequested);
        }

        /// <summary>
        /// Create a response message content based on a requested index in a server batch info
        /// </summary>
        private async Task<HttpMessageSendChangesResponse> GetChangesResponseAsync(HttpContext httpContext, SyncContext syncContext, long remoteClientTimestamp, BatchInfo serverBatchInfo,
                              DatabaseChangesApplied clientChangesApplied, DatabaseChangesSelected serverChangesSelected, int batchIndexRequested)
        {
            var schema = await EnsureSchemaFromSessionAsync(httpContext, syncContext.ScopeName, default, default).ConfigureAwait(false);

            // 1) Create the http message content response
            var changesResponse = new HttpMessageSendChangesResponse(syncContext)
            {
                ServerChangesSelected = serverChangesSelected,
                ClientChangesApplied = clientChangesApplied,
                ServerStep = HttpStep.GetMoreChanges,
                ConflictResolutionPolicy = Options.ConflictResolutionPolicy
            };

            if (serverBatchInfo is null)
                throw new Exception("serverBatchInfo is Null and should not be ....");

            // If nothing to do, just send back
            if (serverBatchInfo.BatchPartsInfo is null || serverBatchInfo.BatchPartsInfo.Count <= 0)
            {
                changesResponse.Changes = new ContainerSet();
                changesResponse.BatchIndex = 0;
                changesResponse.BatchCount = serverBatchInfo.BatchPartsInfo is null ? 0 : serverBatchInfo.BatchPartsInfo.Count;
                changesResponse.IsLastBatch = true;
                changesResponse.RemoteClientTimestamp = remoteClientTimestamp;
                return changesResponse;
            }

            // Get the batch part index requested
            var batchPartInfo = serverBatchInfo.BatchPartsInfo.First(d => d.Index == batchIndexRequested);

            // Get the updatable schema for the only table contained in the batchpartinfo
            var schemaTable = DbSyncAdapter.CreateChangesTable(schema.Tables[batchPartInfo.Tables[0].TableName, batchPartInfo.Tables[0].SchemaName]);

            // Generate the ContainerSet containing rows to send to the user
            var containerSet = new ContainerSet();
            var containerTable = new ContainerTable(schemaTable);
            var fullPath = Path.Combine(serverBatchInfo.GetDirectoryFullPath(), batchPartInfo.FileName);
            containerSet.Tables.Add(containerTable);

            // read rows from file
            var localSerializer = Options.LocalSerializerFactory.GetLocalSerializer();
            foreach (var row in localSerializer.ReadRowsFromFile(fullPath, schemaTable))
                containerTable.Rows.Add(row.ToArray());

            // if client request a conversion on each row, apply the conversion
            if (ClientConverter is not null && containerTable.HasRows)
                BeforeSerializeRows(containerTable, schemaTable, ClientConverter);

            // generate the response
            changesResponse.Changes = containerSet;
            changesResponse.BatchIndex = batchIndexRequested;
            changesResponse.BatchCount = serverBatchInfo.BatchPartsInfo.Count;
            changesResponse.IsLastBatch = batchPartInfo.IsLastBatch;
            changesResponse.RemoteClientTimestamp = remoteClientTimestamp;
            changesResponse.ServerStep = batchPartInfo.IsLastBatch ? HttpStep.GetMoreChanges : HttpStep.GetChangesInProgress;

            return changesResponse;
        }


        /// <summary>
        /// This method is only used when batch mode is enabled on server and we need send to the server the order to delete tmp folder 
        /// </summary>
        internal async Task<HttpMessageSendChangesResponse> SendEndDownloadChangesAsync(HttpContext httpContext, HttpMessageGetMoreChangesRequest httpMessage,
            SessionCache sessionCache, CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            var batchPartInfo = sessionCache.ServerBatchInfo.BatchPartsInfo.First(d => d.Index == httpMessage.BatchIndexRequested);

            // If we have only one bpi, we can safely delete it
            if (batchPartInfo.IsLastBatch)
            {
                // delete the folder (not the BatchPartInfo, because we have a reference on it)
                var cleanFolder = Options.CleanFolder;

                if (cleanFolder)
                    cleanFolder = await InternalCanCleanFolderAsync(httpMessage.SyncContext, sessionCache.ServerBatchInfo, default).ConfigureAwait(false);

                if (cleanFolder)
                    sessionCache.ServerBatchInfo.TryRemoveDirectory();
            }
            return new HttpMessageSendChangesResponse(httpMessage.SyncContext) { ServerStep = HttpStep.SendEndDownloadChanges };
        }


        /// <summary>
        /// Before serializing all rows, call the converter for each row
        /// </summary>
        public void BeforeSerializeRows(ContainerTable table, SyncTable schemaTable, IConverter converter)
        {
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                    converter.BeforeSerialize(row, schemaTable);
            }
        }

        /// <summary>
        /// After deserializing all rows, call the converter for each row
        /// </summary>
        public void AfterDeserializedRows(ContainerTable table, SyncTable schemaTable, IConverter converter)
        {
            if (table.Rows.Count > 0)
            {
                foreach (var row in table.Rows)
                    converter.AfterDeserialized(row, schemaTable);
            }
        }


        /// <summary>
        /// Write exception to output message
        /// </summary>
        public async Task WriteExceptionAsync(HttpRequest httpRequest, HttpResponse httpResponse, Exception ex, string additionalInfo = null)
        {
            // Check if it's an unknown error, not managed (yet)
            if (!(ex is SyncException syncException))
                syncException = new SyncException(ex);

            var message = new StringBuilder();
            message.AppendLine(syncException.Message);
            message.AppendLine("-----------------------");
            message.AppendLine(syncException.StackTrace);
            message.AppendLine("-----------------------");
            if (syncException.InnerException is not null)
            {
                message.AppendLine("-----------------------");
                message.AppendLine("INNER EXCEPTION");
                message.AppendLine("-----------------------");
                message.AppendLine(syncException.InnerException.Message);
                message.AppendLine("-----------------------");
                message.AppendLine(syncException.InnerException.StackTrace);
                message.AppendLine("-----------------------");

            }
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                message.AppendLine("-----------------------");
                message.AppendLine("ADDITIONAL INFO");
                message.AppendLine("-----------------------");
                message.AppendLine(additionalInfo);
                message.AppendLine("-----------------------");

            }

            var webException = new WebSyncException
            {
                Message = message.ToString(),
                SyncStage = syncException.SyncStage,
                TypeName = syncException.TypeName,
                DataSource = syncException.DataSource,
                InitialCatalog = syncException.InitialCatalog,
                Number = syncException.Number,
                Side = syncException.Side
            };

            var jobject = JObject.FromObject(webException);

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);
            using var jtw = new JsonTextWriter(sw);

#if DEBUG
            jtw.Formatting = Formatting.Indented;
#endif
            await jobject.WriteToAsync(jtw);

            await jtw.FlushAsync();
            await sw.FlushAsync();

            var data = ms.ToArray();

            // data to send back, as the response
            byte[] compressedData = EnsureCompression(httpRequest, httpResponse, data);

            httpResponse.Headers.Add("isynergy-sync-error", syncException.TypeName);
            httpResponse.StatusCode = StatusCodes.Status400BadRequest;
            httpResponse.ContentLength = compressedData.Length;
            await httpResponse.Body.WriteAsync(compressedData, 0, compressedData.Length, default).ConfigureAwait(false);

        }

        public static Task WriteHelloAsync(HttpContext context, WebServerOrchestrator orchestrator, CancellationToken cancellationToken = default)
                    => WriteHelloAsync(context, new[] { orchestrator }, cancellationToken);

        public static async Task WriteHelloAsync(HttpContext context, IEnumerable<WebServerOrchestrator> orchestrators, CancellationToken cancellationToken = default)
        {
            var httpResponse = context.Response;
            var stringBuilder = new StringBuilder();


            stringBuilder.AppendLine("<!doctype html>");
            stringBuilder.AppendLine("<html>");
            stringBuilder.AppendLine("<head>");
            stringBuilder.AppendLine("<meta charset='utf-8'>");
            stringBuilder.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1, shrink-to-fit=no'>");
            stringBuilder.AppendLine("<script src='https://cdn.jsdelivr.net/gh/google/code-prettify@master/loader/run_prettify.js'></script>");
            stringBuilder.AppendLine("<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css' integrity='sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh' crossorigin='anonymous'>");
            stringBuilder.AppendLine("</head>");
            stringBuilder.AppendLine("<title>Web Server properties</title>");
            stringBuilder.AppendLine("<body>");


            stringBuilder.AppendLine("<div class='container'>");
            stringBuilder.AppendLine("<h2>Web Server properties</h2>");

            foreach (var webOrchestrator in orchestrators)
            {

                SyncContext ctx = null;
                string dbName = null;
                string version = null;
                string exceptionMessage = null;
                bool hasException = false;
                try
                {
                    (ctx, dbName, version) = await webOrchestrator.GetHelloAsync();
                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                    hasException = true;

                }

                stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                stringBuilder.AppendLine($"<li class='list-group-item active'>Trying to reach database</li>");
                stringBuilder.AppendLine("</ul>");
                if (hasException)
                {
                    stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Exception occured</li>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-danger'>");
                    stringBuilder.AppendLine($"{exceptionMessage}");
                    stringBuilder.AppendLine("</li>");
                    stringBuilder.AppendLine("</ul>");
                }
                else
                {
                    stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Database</li>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-light'>");
                    stringBuilder.AppendLine($"Check database {dbName}: Done.");
                    stringBuilder.AppendLine("</li>");
                    stringBuilder.AppendLine("</ul>");

                    stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Engine version</li>");
                    stringBuilder.AppendLine($"<li class='list-group-item list-group-item-light'>");
                    stringBuilder.AppendLine($"{version}");
                    stringBuilder.AppendLine("</li>");
                    stringBuilder.AppendLine("</ul>");
                }

                stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                stringBuilder.AppendLine($"<li class='list-group-item active'>ScopeName: {webOrchestrator.ScopeName}</li>");
                stringBuilder.AppendLine("</ul>");

                var s = JsonConvert.SerializeObject(webOrchestrator.Setup, Formatting.Indented);
                stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Setup</li>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-light'>");
                stringBuilder.AppendLine("<pre class='prettyprint' style='border:0px;font-size:75%'>");
                stringBuilder.AppendLine(s);
                stringBuilder.AppendLine("</pre>");
                stringBuilder.AppendLine("</li>");
                stringBuilder.AppendLine("</ul>");

                s = JsonConvert.SerializeObject(webOrchestrator.Provider, Formatting.Indented);
                stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Provider</li>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-light'>");
                stringBuilder.AppendLine("<pre class='prettyprint' style='border:0px;font-size:75%'>");
                stringBuilder.AppendLine(s);
                stringBuilder.AppendLine("</pre>");
                stringBuilder.AppendLine("</li>");
                stringBuilder.AppendLine("</ul>");

                s = JsonConvert.SerializeObject(webOrchestrator.Options, Formatting.Indented);
                stringBuilder.AppendLine("<ul class='list-group mb-2'>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-primary'>Options</li>");
                stringBuilder.AppendLine($"<li class='list-group-item list-group-item-light'>");
                stringBuilder.AppendLine("<pre class='prettyprint' style='border:0px;font-size:75%'>");
                stringBuilder.AppendLine(s);
                stringBuilder.AppendLine("</pre>");
                stringBuilder.AppendLine("</li>");
                stringBuilder.AppendLine("</ul>");
            }

            stringBuilder.AppendLine("</div>");
            stringBuilder.AppendLine("</body>");
            stringBuilder.AppendLine("</html>");

            await httpResponse.WriteAsync(stringBuilder.ToString(), cancellationToken);
        }

        /// <summary>
        /// Retrieves the remote folder for the client.
        /// </summary>
        /// <returns></returns>
        public string GetRemoteFullPath(string path)
        {
            Argument.IsNotNullOrEmpty(path);
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Gets a list of files in the remote folder.
        /// </summary>
        /// <returns></returns>
        public List<FileInfoMetadata> GetRemoteFiles(string path)
        {
            Argument.IsNotNullOrEmpty(path);
            var remotePath = GetRemoteFullPath(path);
            var remoteFolder = new DirectoryInfo(remotePath);
            return remoteFolder
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Select(s => new FileInfoMetadata(remoteFolder.FullName, s))
                .ToList();
        }

        /// <summary>
        /// Downloads file by it's full name.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>PhysicalFileResult</returns>
        public PhysicalFileResult DownloadFile(string path) =>
            new PhysicalFileResult(path, "application/octet-stream");
    }
}
