using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class CameraService.
/// </summary>
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
        var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        var device = devices.FirstOrDefault(); // Finds one device, my webcam
        if (device is not null)
        {
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
                        () => photo.Frame.AsStreamForRead());
                }

                await _dialogService.ShowErrorAsync(string.Format(LanguageService.Default.GetString("WarningDocumentSizeTooBig"), $"{maxFileSize} bytes"));
            }
        }

        return null;
    }
}
