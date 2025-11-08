using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
    /// <returns>FileResult.</returns>
    public async Task<FileResult> TakePictureAsync(long maxFileSize = 1 * 1024 * 1024)
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Png;
        captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
        captureUI.PhotoSettings.AllowCropping = false;

        if (await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo) is StorageFile photo)
        {
            var prop = await photo.GetBasicPropertiesAsync();

            return new FileResult(
                    photo.Path,
                    () => photo.OpenStreamForReadAsync().GetAwaiter().GetResult());
        }

        return null;
    }
}
