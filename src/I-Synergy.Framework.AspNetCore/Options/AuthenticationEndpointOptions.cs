namespace ISynergy.Framework.AspNetCore.Options
{
    public class AuthenticationEndpointOptions
    {
        public string TokenEndpointPath { get; set; } = string.Empty;
        public string AuthorizationEndpointPath { get; set; } = string.Empty;
        public string LogoutEndpointPath { get; set; } = string.Empty;
        public string UserinfoEndpointPath { get; set; } = string.Empty;
    }
}
