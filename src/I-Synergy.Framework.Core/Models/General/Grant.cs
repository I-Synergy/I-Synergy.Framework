namespace ISynergy.Models.General
{
    public class Grant
    {
        public string Grant_Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Refresh_Token { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
        public string Scope { get; set; }
    }

    public static class Grant_Types
    {
        public const string password = "password";
        public const string refresh_token = "refresh_token";
        public const string client_credentials = "client_credentials";
    }

    public static class Grant_Scopes
    {
        public const string password = "openid offline_access";
        public const string client_credentials = "openid";
    }
}