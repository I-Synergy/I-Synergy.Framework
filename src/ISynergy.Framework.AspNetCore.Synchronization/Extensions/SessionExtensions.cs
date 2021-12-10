using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace ISynergy.Framework.AspNetCore.Synchronization.Extensions
{

    public static class SessionExtensions
    {
        public static T Get<T>(this ISession session, string key)
        {
            var data = session.GetString(key);

            if (data == null)
                return default;

            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            var jsonValue = JsonConvert.SerializeObject(value);

            session.SetString(key, jsonValue);
        }
    }
}
