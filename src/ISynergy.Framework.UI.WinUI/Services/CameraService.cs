using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

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
        var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        DeviceInformation device = devices.FirstOrDefault(); // Finds one device, my webcam

        var settings = new MediaCaptureInitializationSettings();
        settings.VideoDeviceId = device.Id;
        settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        settings.PhotoCaptureSource = PhotoCaptureSource.Photo;

        var mediaCapture = new MediaCapture();
        await mediaCapture.InitializeAsync(settings);

        var capture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreatePng()).AsTask();
        var photo = await capture.CaptureAsync().AsTask();
        var bitmap = photo.Frame.SoftwareBitmap;

        if (photo.Frame is not null)
        {
            if (photo.Frame.Size.ToLong() <= maxFileSize)
            {
                var fileName = $"Capture {DateTime.Now}";
                return new FileResult(
                    $"{fileName}.png",
                    fileName,
                    () => photo.Frame.AsStreamForRead());
            }

            await _dialogService.ShowErrorAsync(string.Format(_languageService.GetString("WarningDocumentSizeTooBig"), $"{maxFileSize} bytes"));
        }

        return null;
    }
}
