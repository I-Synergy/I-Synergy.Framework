using Newtonsoft.Json;

namespace ISynergy.Framework.Core.Models
{
    public class Grant
    {
        [JsonProperty("grant_type")] public string grant_type { get; set; } = string.Empty;
        [JsonProperty("username")] public string username { get; set; } = string.Empty;
        [JsonProperty("password")] public string password { get; set; } = string.Empty;
        [JsonProperty("refresh_token")] public string refresh_token { get; set; } = string.Empty;
        [JsonProperty("client_id")] public string client_id { get; set; } = string.Empty;
        [JsonProperty("client_secret")] public string client_secret { get; set; } = string.Empty;
        [JsonProperty("scope")] public string scope { get; set; } = string.Empty;
    }

    public static class GrantTypes
    {
        public const string Password = "password";
        public const string RefreshToken = "refresh_token";
        public const string ClientCredentials = "client_credentials";
    }

    public static class GrantScopes
    {
        public const string Password = "openid offline_access";
        public const string ClientCredentials = "openid";
    }
}
