using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ISynergy.Helpers
{
    public static class Json
    {
        public static Task<T> ToObjectAsync<T>(string value)
        {
            return Task.Run<T>(() =>
            {
                return JsonConvert.DeserializeObject<T>(value);
            });
        }

        public static Task<string> StringifyAsync(object value)
        {
            return Task.Run<string>(() =>
            {
                return JsonConvert.SerializeObject(value);
            });
        }
    }
}
