﻿using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client
{

    /// <summary>
    /// Represents a request made to the server to get the server scope info
    /// </summary>
    public class HttpGettingSchemaRequestArgs : ProgressArgs
    {
        public HttpGettingSchemaRequestArgs(SyncContext context, string host) : base(context, null)
        {
            Host = host;
        }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override string Source => Host;
        public override int EventId => HttpClientSyncEventsId.HttpGettingSchemaRequest.Id;
        public override string Message => $"Getting Server Schema. Scope Name:{Context.ScopeName}.";

        public string Host { get; }
    }
    public class HttpGettingSchemaResponseArgs : ProgressArgs
    {
        public HttpGettingSchemaResponseArgs(ServerScopeInfo serverScopeInfo, SyncSet schema, SyncContext context, string host) : base(context, null)
        {
            ServerScopeInfo = serverScopeInfo;
            Schema = schema;
            Host = host;
        }
        public override string Source => Host;
        public override int EventId => HttpClientSyncEventsId.HttpGettingSchemaResponse.Id;
        public override string Message => $"Received Schema From Server. Tables Count:{Schema.Tables.Count}.";

        public SyncSet Schema { get; set; }
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public ServerScopeInfo ServerScopeInfo { get; set; }

        public string Host { get; }
    }

    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpGettingSchemaRequest => new EventId(20200, nameof(HttpGettingSchemaRequest));
        public static EventId HttpGettingSchemaResponse => new EventId(20250, nameof(HttpGettingSchemaResponse));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when an http call is about to be made to get server schema
        /// </summary>
        public static void OnHttpGettingSchemaRequest(this WebClientOrchestrator orchestrator, Action<HttpGettingSchemaRequestArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http call is about to be made to get server schema
        /// </summary>
        public static void OnHttpGettingSchemaRequest(this WebClientOrchestrator orchestrator, Func<HttpGettingSchemaRequestArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http call to get schema is done
        /// </summary>
        public static void OnHttpGettingSchemaResponse(this WebClientOrchestrator orchestrator, Action<HttpGettingSchemaResponseArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http call to get schema is done
        /// </summary>
        public static void OnHttpGettingSchemaResponse(this WebClientOrchestrator orchestrator, Func<HttpGettingSchemaResponseArgs, Task> action)
            => orchestrator.SetInterceptor(action);
    }
}