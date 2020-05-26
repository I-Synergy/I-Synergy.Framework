using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ISynergy.Framework.Windows.Helpers
{
    public static class Json
    {
        public static Task<T> ToObjectAsync<T>(string value)
        {
            return Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }

        public static Task<string> StringifyAsync(object value)
        {
            return Task.Run(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }
    }
}
