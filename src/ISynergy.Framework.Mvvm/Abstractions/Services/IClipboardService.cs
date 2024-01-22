using ISynergy.Framework.Core.Models.Results;

namespace ISynergy.Framework.Mvvm.Abstractions.Services;

/// <summary>
/// Interface IClipboardService
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Gets image (bytes and content type) from clipboard.
    /// </summary>
    /// <returns></returns>
    Task<ImageResult> GetImageFromClipboardAsync();
}
