﻿using ISynergy.Framework.Synchronization.Client.Enumerations;
using ISynergy.Framework.Synchronization.Client.Exceptions;
using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Algorithms;
using ISynergy.Framework.Synchronization.Core.Orchestrators;
using ISynergy.Framework.Synchronization.Core.Serialization;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Client.Handlers
{
    /// <summary>
    /// Object in charge to send requests
    /// </summary>
    public class HttpRequestHandler : IDisposable
    {
        internal Dictionary<string, string> CustomHeaders { get; } = new Dictionary<string, string>();

        internal Dictionary<string, string> ScopeParameters { get; } = new Dictionary<string, string>();

        internal CookieHeaderValue Cookie { get; set; }

        private BaseOrchestrator _orchestrator;

        public HttpRequestHandler(BaseOrchestrator orchestrator)
            => _orchestrator = orchestrator;


        /// <summary>
        /// Process a request message with HttpClient object. 
        /// </summary>
        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpClient client, string baseUri, byte[] data, HttpStep step, Guid sessionId, string scopeName,
            ISerializerFactory serializerFactory, IConverter converter, int batchSize, SyncPolicy policy,
            CancellationToken cancellationToken = default, IProgress<ProgressArgs> progress = null)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            if (baseUri is null)
                throw new ArgumentException("BaseUri is not defined");

            HttpResponseMessage response = null;
            //var responseMessage = default(U);
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                var requestUri = new StringBuilder();
                requestUri.Append(baseUri);
                requestUri.Append(baseUri.EndsWith("/", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : "/");

                // Add params if any
                if (ScopeParameters is not null && ScopeParameters.Count > 0)
                {
                    string prefix = "?";
                    foreach (var kvp in ScopeParameters)
                    {
                        requestUri.AppendFormat("{0}{1}={2}", prefix, Uri.EscapeDataString(kvp.Key),
                                                Uri.EscapeDataString(kvp.Value));
                        if (prefix.Equals("?"))
                            prefix = "&";
                    }
                }

                // Check if data is null
                data = data is null ? new byte[] { } : data;

                // calculate hash
                var hash = HashAlgorithm.SHA256.Create(data);
                var hashString = Convert.ToBase64String(hash);


                string contentType = null;
                // If Json, specify header
                if (serializerFactory.Key == SerializersCollection.JsonSerializerFactory.Key)
                    contentType = "application/json";

                // serialize the serialization format and the batchsize we want.
                var ser = Newtonsoft.Json.JsonConvert.SerializeObject(new { f = serializerFactory.Key, s = batchSize });

                //// Execute my OpenAsync in my policy context
                response = await policy.ExecuteAsync(ct => SendAsync(client, requestUri.ToString(),
                    sessionId.ToString(), scopeName, step, data, ser, converter, hashString, contentType,
                    ct), cancellationToken, progress);


                // try to set the cookie for http session
                var headers = response?.Headers;

                // Ensure we have a cookie
                EnsureCookie(headers);

                if (response.Content is null)
                    throw new HttpEmptyResponseContentException();

                await _orchestrator.InterceptAsync(new HttpGettingResponseMessageArgs(response, _orchestrator.GetContext()), progress, cancellationToken).ConfigureAwait(false);

                return response;
            }
            catch (SyncException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (response is null || response.Content is null)
                    throw new HttpResponseContentException(e.Message);

                var exrror = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                throw new HttpResponseContentException(exrror);

            }

        }

        private void EnsureCookie(HttpResponseHeaders headers)
        {
            if (headers is null)
                return;
            if (!headers.TryGetValues("Set-Cookie", out var tmpList))
                return;

            var cookieList = tmpList.ToList();

            // var cookieList = response.Headers.GetValues("Set-Cookie").ToList();
            if (cookieList is not null && cookieList.Count > 0)
            {
#if NETSTANDARD
                // Get the first cookie
                Cookie = CookieHeaderValue.ParseList(cookieList).FirstOrDefault();
#else
                //try to parse the very first cookie
                if (CookieHeaderValue.TryParse(cookieList[0], out var cookie))
                    Cookie = cookie;
#endif
            }
        }


        private async Task<HttpResponseMessage> SendAsync(HttpClient client, string requestUri,
            string sessionId, string scopeName, HttpStep step,
            byte[] data, string ser, IConverter converter, string hashString, string contentType, CancellationToken cancellationToken)
        {

            // get byte array content
            var arrayContent = new ByteArrayContent(data);

            // Create the request message
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = arrayContent };

            // Adding the serialization format used and session id and scope name
            requestMessage.Headers.Add("isynergy-sync-session-id", sessionId.ToString());
            requestMessage.Headers.Add("isynergy-sync-scope-name", scopeName);
            requestMessage.Headers.Add("isynergy-sync-step", ((int)step).ToString());
            requestMessage.Headers.Add("isynergy-sync-serialization-format", ser);

            // if client specifies a converter, add it as header
            if (converter is not null)
                requestMessage.Headers.Add("isynergy-sync-converter", converter.Key);


            requestMessage.Headers.Add("isynergy-sync-hash", hashString);

            // Adding others headers
            if (CustomHeaders is not null && CustomHeaders.Count > 0)
                foreach (var kvp in CustomHeaders)
                    if (!requestMessage.Headers.Contains(kvp.Key))
                        requestMessage.Headers.Add(kvp.Key, kvp.Value);

            // If Json, specify header
            if (!string.IsNullOrEmpty(contentType) && !requestMessage.Content.Headers.Contains("content-type"))
                requestMessage.Content.Headers.Add("content-type", contentType);

            await _orchestrator.InterceptAsync(new HttpSendingRequestMessageArgs(requestMessage, _orchestrator.GetContext()), progress: default, cancellationToken).ConfigureAwait(false);

            // Eventually, send the request
            var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            //var response = await client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            // throw exception if response is not successfull
            // get response from server
            if (!response.IsSuccessStatusCode && response.Content is not null)
                await HandleSyncError(response);

            return response;

        }

        /// <summary>
        /// Handle a request error
        /// </summary>
        /// <returns></returns>
        private async Task HandleSyncError(HttpResponseMessage response)
        {
            try
            {
                HttpSyncWebException syncException = null;

                if (!TryGetHeaderValue(response.Headers, "isynergy-sync-error", out string syncErrorTypeName))
                {
                    var exceptionString = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(exceptionString))
                        exceptionString = response.ReasonPhrase;

                    syncException = new HttpSyncWebException(exceptionString);
                }
                else
                {
                    using var streamResponse = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    if (streamResponse.CanRead)
                    {
                        // Error are always json formatted
                        var webSyncErrorSerializer = new JsonConverter<WebSyncException>();
                        var webError = await webSyncErrorSerializer.DeserializeAsync(streamResponse);

                        if (webError is not null)
                        {
                            var exceptionString = webError.Message;

                            if (string.IsNullOrEmpty(exceptionString))
                                exceptionString = response.ReasonPhrase;

                            syncException = new HttpSyncWebException(exceptionString)
                            {
                                DataSource = webError.DataSource,
                                InitialCatalog = webError.InitialCatalog,
                                Number = webError.Number,
                                Side = webError.Side,
                                SyncStage = webError.SyncStage,
                                TypeName = webError.TypeName
                            };
                        }
                        else
                        {
                            syncException = new HttpSyncWebException(response.ReasonPhrase);
                        }

                    }

                }

                syncException.ReasonPhrase = response.ReasonPhrase;
                syncException.StatusCode = response.StatusCode;

                throw syncException;


            }
            catch (SyncException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SyncException(ex);
            }


        }



        public static bool TryGetHeaderValue(HttpResponseHeaders n, string key, out string header)
        {
            if (n.TryGetValues(key, out var vs))
            {
                header = vs.First();
                return true;
            }

            header = null;
            return false;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion


    }
}