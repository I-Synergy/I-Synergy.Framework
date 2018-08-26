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

    public class Grant_Types
    {
        public const string password = "password";
        public const string refresh_token = "refresh_token";
        public const string client_credentials = "client_credentials";
    }

    public class Grant_Scopes
    {
        public const string password = "openid offline_access";
        public const string client_credentials = "openid";
    }
}