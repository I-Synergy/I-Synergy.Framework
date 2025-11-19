using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Services;

public class FileService : IFileService<FileResult>
{
    public async Task<List<FileResult>> BrowseFileAsync(string filefilter, bool multiple = false, long maxFileSize = 1048576)
    {
        var result = new List<FileResult>();

        try
        {
            var filter = filefilter.Split(';');
            var pickOptions = new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, filter },
                    { DevicePlatform.Android, filter },
                    { DevicePlatform.WinUI, filter }
                })
            };

            if (multiple)
            {
                var files = await FilePicker.Default.PickMultipleAsync(pickOptions);

                foreach (var file in files.EnsureNotNull())
                {
                    if (file is not null)
                    {
                        var fileResult = file.ToFileResult();

                        if (fileResult is not null)
                            result.Add(fileResult);
                    }
                }
            }
            else
            {
                var file = await FilePicker.Default.PickAsync(pickOptions);

                if (file is not null)
                {
                    var fileResult = file.ToFileResult();

                    if (fileResult is not null)
                        result.Add(fileResult);
                }
            }
        }
        catch (Exception)
        {
            // The user canceled or something went wrong
        }

        return result;
    }

    public async Task<byte[]?> BrowseImageAsync(string[] filter, long maxFileSize = 1048576)
    {
        if (await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is { } result)
            return result[0].File;

        return null;
    }

    public Task OpenFileAsync(string fileToOpen) =>
        Launcher.Default.OpenAsync(new OpenFileRequest("", new ReadOnlyFile(fileToOpen)));

    /// <summary>
    /// Saves file to folder async.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="filename"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<FileResult?> SaveFileAsync(string folder, string filename, byte[] file)
    {
        throw new NotImplementedException();
    }
}
