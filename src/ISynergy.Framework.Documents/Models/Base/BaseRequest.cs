namespace ISynergy.Framework.Documents.Models.Base;

/// <summary>
/// Base report request.
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName { get; set; } = string.Empty;
}
