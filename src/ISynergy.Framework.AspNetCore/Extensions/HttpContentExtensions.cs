using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Extensions
{
    /// <summary>
    /// Class HttpContentExtensions.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// read as as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content">The content.</param>
        /// <returns>T.</returns>
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
