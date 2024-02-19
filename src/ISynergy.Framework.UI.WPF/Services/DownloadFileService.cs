﻿using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using System.Diagnostics;

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
            Process.Start(savedFile.FilePath);
        }
    }
}
