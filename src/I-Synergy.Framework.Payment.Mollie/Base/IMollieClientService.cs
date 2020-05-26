using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ISynergy.Framework.Payment.Abstractions;
using ISynergy.Framework.Payment.Mollie.Models.Url;

namespace ISynergy.Framework.Payment.Mollie.Base
{
    /// <summary>
    /// Interface IMollieClientService
    /// Implements the <see cref="IClientService" />
    /// </summary>
    /// <seealso cref="IClientService" />
    public interface IMollieClientService : IClientService
    {
        /// <summary>
        /// Sends the HTTP request asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> SendHttpRequestAsync<T>(HttpMethod httpMethod, string relativeUri, object data = null);
        /// <summary>
        /// Gets the list asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="from">From.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="otherParameters">The other parameters.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> GetListAsync<T>(string relativeUri, string from, int? limit, IDictionary<string, string> otherParameters = null);
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> GetAsync<T>(string relativeUri);
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlObject">The URL object.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> GetAsync<T>(UrlObjectLink<T> urlObject);
        /// <summary>
        /// Posts the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> PostAsync<T>(string relativeUri, object data);
        /// <summary>
        /// Patches the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        Task<T> PatchAsync<T>(string relativeUri, object data);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="data">The data.</param>
        /// <returns>Task.</returns>
        Task DeleteAsync(string relativeUri, object data = null);
    }
}
