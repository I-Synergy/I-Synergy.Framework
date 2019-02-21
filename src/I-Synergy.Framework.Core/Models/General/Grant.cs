namespace ISynergy.Models.General
{
    public class Grant
    {
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string scope { get; set; }
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