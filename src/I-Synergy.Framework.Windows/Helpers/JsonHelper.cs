using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ISynergy.Framework.Windows.Helpers
{
    /// <summary>
    /// Class Json.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Converts to objectasync.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> ToObjectAsync<T>(string value)
        {
            return Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }

        /// <summary>
        /// Stringifies the asynchronous.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static Task<string> StringifyAsync(object value)
        {
            return Task.Run(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }
    }
}
