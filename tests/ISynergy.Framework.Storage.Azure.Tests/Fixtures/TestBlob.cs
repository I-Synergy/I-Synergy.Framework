namespace ISynergy.Framework.Storage.Azure.Tests.Fixtures;

/// <summary>
/// Test blob data for BDD scenarios.
/// </summary>
public class TestBlob
{
    public string Name { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string ContentType { get; set; } = "application/octet-stream";
    public long Size => Content.Length;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string Uri { get; set; } = string.Empty;
    public string SasToken { get; set; } = string.Empty;
}
