using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Scopes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client.Arguments
{

    /// <summary>
    /// Represents a response from server containing the server scope info
    /// </summary>
    public class HttpGettingScopeResponseArgs : ProgressArgs
    {
        public HttpGettingScopeResponseArgs(ServerScopeInfo scopeInfo, SyncContext context, string host) : base(context, null)
        {
            ServerScopeInfo = scopeInfo;
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => HttpClientSyncEventsId.HttpGettingScopeResponse.Id;
        public override string Source => Host;
        public override string Message => $"Received Scope. Scope Name:{ServerScopeInfo.Name}.";

        public ServerScopeInfo ServerScopeInfo { get; }
        public string Host { get; }
    }

    /// <summary>
    /// Represents a request made to the server to get the server scope info
    /// </summary>
    public class HttpGettingScopeRequestArgs : ProgressArgs
    {
        public HttpGettingScopeRequestArgs(SyncContext context, string host) : base(context, null)
        {
            Host = host;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => HttpClientSyncEventsId.HttpGettingScopeRequest.Id;
        public override string Source => Host;
        public override string Message => $"Getting Server Scope. Scope Name:{Context.ScopeName}.";

        public string Host { get; }
    }

    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpGettingScopeRequest => new EventId(20300, nameof(HttpGettingScopeRequest));
        public static EventId HttpGettingScopeResponse => new EventId(20350, nameof(HttpGettingScopeResponse));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when an http is about to be done to get server scope 
        /// </summary>
        public static void OnHttpGettingScopeRequest(this WebClientOrchestrator orchestrator, Action<HttpGettingScopeRequestArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http is about to be done to get server scope 
        /// </summary>
        public static void OnHttpGettingScopeRequest(this WebClientOrchestrator orchestrator, Func<HttpGettingScopeRequestArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http call to get scope is done
        /// </summary>
        public static void OnHttpGettingScopeResponse(this WebClientOrchestrator orchestrator, Action<HttpGettingScopeResponseArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http call to get scope is done
        /// </summary>
        public static void OnHttpGettingScopeResponse(this WebClientOrchestrator orchestrator, Func<HttpGettingScopeResponseArgs, Task> action)
            => orchestrator.SetInterceptor(action);


    }
}
