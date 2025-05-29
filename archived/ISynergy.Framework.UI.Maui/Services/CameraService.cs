using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;

namespace ISynergy.Framework.UI.Services;

public class CameraService : ICameraService
{
    private readonly IDialogService _dialogService;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraService"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="logger"></param>
    public CameraService(
        IDialogService dialogService,
        ILogger<CameraService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"CameraService instance created with ID: {Guid.NewGuid()}");

        _dialogService = dialogService;
    }

    /// <summary>
    /// take picture as an asynchronous operation.
    /// </summary>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>FileResult.</returns>
    public async Task<FileResult?> TakePictureAsync(long maxFileSize = 1 * 1024 * 1024)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null && photo.ToFileResult() is FileResult result)
            {
                if (result.File.Length <= maxFileSize)
                    return result;

                await _dialogService.ShowErrorAsync(string.Format(LanguageService.Default.GetString("WarningDocumentSizeTooBig"), $"{maxFileSize} bytes"));
            }
        }

        return null;
    }
}
