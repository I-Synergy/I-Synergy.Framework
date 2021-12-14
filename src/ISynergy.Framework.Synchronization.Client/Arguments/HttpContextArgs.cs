﻿using ISynergy.Framework.Synchronization.Client.Orchestrators;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Arguments;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client.Arguments
{

    /// <summary>
    /// When Getting response from remote orchestrator
    /// </summary>
    public class HttpGettingResponseMessageArgs : ProgressArgs
    {
        public HttpGettingResponseMessageArgs(HttpResponseMessage response, SyncContext context)
            : base(context, null, null)
        {
            Response = response;
        }

        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public override int EventId => HttpClientSyncEventsId.HttpGettingResponseMessage.Id;

        public HttpResponseMessage Response { get; }
    }

    /// <summary>
    /// When sending request 
    /// </summary>
    public class HttpSendingRequestMessageArgs : ProgressArgs
    {
        public HttpSendingRequestMessageArgs(HttpRequestMessage request, SyncContext context)
            : base(context, null, null)
        {
            Request = request;
        }

        public override int EventId => HttpClientSyncEventsId.HttpSendingRequestMessage.Id;
        public override SyncProgressLevel ProgressLevel => SyncProgressLevel.Debug;
        public HttpRequestMessage Request { get; }
    }

    public static partial class HttpClientSyncEventsId
    {
        public static EventId HttpSendingRequestMessage => new EventId(20100, nameof(HttpSendingRequestMessage));
        public static EventId HttpGettingResponseMessage => new EventId(20150, nameof(HttpGettingResponseMessage));
    }

    /// <summary>
    /// Partial interceptors extensions 
    /// </summary>
    public static partial class HttpInterceptorsExtensions
    {
        /// <summary>
        /// Intercept the provider when an http request message is sent
        /// </summary>
        public static void OnHttpSendingRequest(this WebClientOrchestrator orchestrator, 
            Action<HttpSendingRequestMessageArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when an http request message is sent
        /// </summary>
        public static void OnHttpSendingRequest(this WebClientOrchestrator orchestrator, 
            Func<HttpSendingRequestMessageArgs, Task> action)
            => orchestrator.SetInterceptor(action);

        /// <summary>
        /// Intercept the provider when an http message response is downloaded from remote side
        /// </summary>
        public static void OnHttpGettingResponse(this WebClientOrchestrator orchestrator, 
            Action<HttpGettingResponseMessageArgs> action)
            => orchestrator.SetInterceptor(action);
        /// <summary>
        /// Intercept the provider when an http message response is downloaded from remote side
        /// </summary>
        public static void OnHttpGettingResponse(this WebClientOrchestrator orchestrator, 
            Func<HttpGettingResponseMessageArgs, Task> action)
            => orchestrator.SetInterceptor(action);

    }
}
