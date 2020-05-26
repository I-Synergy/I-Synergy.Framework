namespace ISynergy.Framework.AspNetCore.Authentication.Options
{
    public class JwtOptions
    {
        public string SymmetricKeySecret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
