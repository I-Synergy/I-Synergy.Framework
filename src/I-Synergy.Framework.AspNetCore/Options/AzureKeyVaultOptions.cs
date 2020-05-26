namespace ISynergy.Framework.AspNetCore.Options
{
    public class AzureKeyVaultOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string KeyUri { get; set; } = string.Empty;
    }
}