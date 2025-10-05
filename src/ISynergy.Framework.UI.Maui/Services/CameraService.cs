using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Services;

public class CameraService : ICameraService
{
    public async Task<FileResult> TakePictureAsync(long maxFileSize = 1048576)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null && photo.ToFileResult() is FileResult result && result.File.Length <= maxFileSize)
                return result;
        }

        return null;
    }
}
