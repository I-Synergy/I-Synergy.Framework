using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using System.Diagnostics;
using System.IO;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class DownloadFileService.
/// </summary>
public class DownloadFileService : IDownloadFileService
{
    /// <summary>
    /// The file service
    /// </summary>
    private readonly IFileService<FileResult> _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadFileService"/> class.
    /// </summary>
    /// <param name="fileservice">The fileservice.</param>
    public DownloadFileService(IFileService<FileResult> fileservice)
    {
        _fileService = fileservice;
    }

    /// <summary>
    /// download file as an asynchronous operation.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="file">The file.</param>
    /// <param name="filename">The filename.</param>
    public async Task DownloadFileAsync(string folder, string filename, byte[] file)
    {
        if (await _fileService.SaveFileAsync(folder, filename, file) is { } savedFile)
        {
            // Normalize and verify the saved path before opening to prevent path-traversal attacks.
            var normalizedPath = Path.GetFullPath(savedFile.FilePath);

            if (File.Exists(normalizedPath))
            {
                // UseShellExecute = true is required on Windows to open the file with its default
                // associated application. The path was produced by a SaveFileDialog and has been
                // normalized, so it is safe to pass to the shell.
                Process.Start(new ProcessStartInfo(normalizedPath) { UseShellExecute = true });
            }
        }
    }
}
