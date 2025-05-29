#if !WINDOWS
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class ClipboardService.
/// </summary>
public class ClipboardService : IClipboardService
{
    private readonly ILogger _logger;

    public ClipboardService(ILogger<ClipboardService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"ClipboardService instance created with ID: {Guid.NewGuid()}");
    }

    public Task<ImageResult?> GetImageFromClipboardAsync()
    {
        return Task.FromResult<ImageResult?>(null);
    }
}
#endif