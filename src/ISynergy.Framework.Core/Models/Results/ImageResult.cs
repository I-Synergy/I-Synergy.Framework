namespace ISynergy.Framework.Core.Models.Results;

public sealed class ImageResult
{
    public byte[] FileBytes { get; }
    public string ContentType { get; }

    public ImageResult(byte[] fileBytes, string contentType)
    {
        FileBytes = fileBytes;
        ContentType = contentType;
    }
}
