using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    internal class FileService : IFileService<FileResult>
    {
        public string FileName { get; set; }
        public string Filter { get; set; }
        public bool AddExtension { get; set; }
        public bool CheckFileExists { get; set; }
        public bool CheckPathExists { get; set; }
        public int FilterIndex { get; set; }
        public string InitialDirectory { get; set; }
        public string Title { get; set; }
        public bool ValidateNames { get; set; }

        public Task<FileResult> BrowseFileAsync(string filefilter, long maxFileSize = 1048576)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> BrowseImageAsync(string[] filter, long maxFileSize = 1048576)
        {
            throw new NotImplementedException();
        }

        public Task OpenFileAsync(string fileToOpen)
        {
            throw new NotImplementedException();
        }

        public Task<FileResult> SaveFileAsync(string filename, byte[] file)
        {
            throw new NotImplementedException();
        }
    }
}
