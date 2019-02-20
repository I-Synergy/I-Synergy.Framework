namespace ISynergy.Models.General
{
#pragma warning disable IDE1006 // Naming Styles
// Reason: Class is used as payload of a HTTP form post.
// Flurl uses the capitalization of the property names as post parameter keys.
// There's currently no way to customize this with, for example, an attribute.
    public class Token
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}