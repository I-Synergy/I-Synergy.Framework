using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    internal class FileService : IFileService<FileResult>
    {
        public string Filter { get; set; }
        public bool AddExtension { get; set; }
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public int FilterIndex { get; set; }
        public string InitialDirectory { get; set; }
        public string Title { get; set; }
        public bool ValidateNames { get; set; }

        public async Task<List<FileResult>> BrowseFileAsync(string filefilter, bool multiple = false, long maxFileSize = 1048576)
        {
            try
            {
                var result = new List<FileResult>();
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
                    result.AddRange(await FilePicker.Default.PickMultipleAsync(pickOptions));
                else
                    result.Add(await FilePicker.Default.PickAsync(pickOptions));

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public async Task<byte[]> BrowseImageAsync(string[] filter, long maxFileSize = 1048576)
        {
            if (await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is List<FileResult> result)
            {
                using var stream = await result.First().OpenReadAsync();
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }

            return null;
        }

        public Task OpenFileAsync(string fileToOpen) =>
            Launcher.Default.OpenAsync(new OpenFileRequest("", new ReadOnlyFile(fileToOpen)));

        public Task<FileResult> SaveFileAsync(string filename, byte[] file)
        {
            throw new NotImplementedException();
        }
    }
}
