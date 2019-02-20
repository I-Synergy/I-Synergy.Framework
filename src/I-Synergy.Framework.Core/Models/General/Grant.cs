namespace ISynergy.Models.General
{
#pragma warning disable IDE1006 // Naming Styles
    // Reason: Class is used as payload of a HTTP form post.
    // Flurl uses the capitalization of the property names as post parameter keys.
    // There's currently no way to customize this with, for example, an attribute.
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
#pragma warning restore IDE1006 // Naming Styles

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