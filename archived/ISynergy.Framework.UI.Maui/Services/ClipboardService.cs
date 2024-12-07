#if !WINDOWS
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ClipboardService.
/// </summary>
public class ClipboardService : IClipboardService
{
    public Task<ImageResult> GetImageFromClipboardAsync()
    {
        return Task.FromResult<ImageResult>(null);
    }
}
#endif