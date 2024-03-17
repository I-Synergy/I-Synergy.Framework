using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.Media.Capture;
using Windows.Storage;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class CameraService.
/// </summary>
public class CameraService : ICameraService
{
    /// <summary>
    /// The dialog service
    /// </summary>
    private readonly IDialogService _dialogService;
    /// <summary>
    /// The language service
    /// </summary>
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraService"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="languageService">The language service.</param>
    public CameraService(IDialogService dialogService, ILanguageService languageService)
    {
        _dialogService = dialogService;
        _languageService = languageService;
    }

    /// <summary>
    /// take picture as an asynchronous operation.
    /// </summary>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>FileResult.</returns>
    public async Task<FileResult> TakePictureAsync(long maxFileSize = 1 * 1024 * 1024)
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Png;
        captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.MediumXga;
        captureUI.PhotoSettings.AllowCropping = false;

        if (await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo) is StorageFile photo)
        {
            var prop = await photo.GetBasicPropertiesAsync();

            if (prop.Size.ToLong() <= maxFileSize)
            {
                return new FileResult(
                    photo.Path,
                    photo.DisplayName,
                    () => photo.OpenStreamForReadAsync().GetAwaiter().GetResult());
            }
            else
            {
                await _dialogService.ShowErrorAsync(string.Format(_languageService.GetString("Warning_Document_SizeTooBig"), $"{maxFileSize} bytes"));
            }
        }

        return null;
    }
}
