using ISynergy.Framework.AspNetCore.Synchronization.Arguments;
using ISynergy.Framework.AspNetCore.Synchronization.Orchestrators;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Synchronization.Extensions
{
    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpServerInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when an http response message is sent back to the client
        /// </summary>
        public static void OnHttpSendingResponse(this WebServerOrchestrator orchestrator,
            Action<HttpSendingResponseArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http response message is sent back to the client
        /// </summary>
        public static void OnHttpSendingResponse(this WebServerOrchestrator orchestrator,
            Func<HttpSendingResponseArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http message request from the client arrived to the server
        /// </summary>
        public static void OnHttpGettingRequest(this WebServerOrchestrator orchestrator,
            Action<HttpGettingRequestArgs> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http message request from the client arrived to the server
        /// </summary>
        public static void OnHttpGettingRequest(this WebServerOrchestrator orchestrator,
            Func<HttpGettingRequestArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http message is sent
        /// </summary>
        public static void OnHttpSendingChanges(this WebServerOrchestrator orchestrator,
            Action<HttpSendingServerChangesArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when an http message is sent
        /// </summary>
        public static void OnHttpSendingChanges(this WebServerOrchestrator orchestrator,
            Func<HttpSendingServerChangesArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http message is downloaded from remote side
        /// </summary>
        public static void OnHttpGettingChanges(this WebServerOrchestrator orchestrator,
            Action<HttpGettingClientChangesArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when an http message is downloaded from remote side
        /// </summary>
        public static void OnHttpGettingChanges(this WebServerOrchestrator orchestrator,
            Func<HttpGettingClientChangesArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }
}
