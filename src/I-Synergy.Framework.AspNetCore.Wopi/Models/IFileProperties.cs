namespace ISynergy.Framework.Wopi.Models
{
    public interface IFileProperties
    {
        string BaseFileName { get; set; }
        string OwnerId { get; set; }
        long Size { get; set; }
        string UserId { get; set; }
        string Version { get; set; }
        string FileExtension { get; set; }
        string LastModifiedTime { get; set; }
        string SHA256 { get; set; }
        string UniqueContentId { get; set; }
    }
}
