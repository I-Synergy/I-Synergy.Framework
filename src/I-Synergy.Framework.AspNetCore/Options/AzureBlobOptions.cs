namespace ISynergy.Framework.AspNetCore.Options
{
    public class AzureBlobOptions
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
    }

    public class AzureDocumentOptions
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
    }
}