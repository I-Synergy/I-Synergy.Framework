﻿using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.Extensions.Logging;
using Windows.Storage;
using Windows.System;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Class DownloadFileService.
/// </summary>
public class DownloadFileService : IDownloadFileService
{
    private readonly ILogger _logger;
    private readonly IFileService<FileResult> _fileService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadFileService"/> class.
    /// </summary>
    /// <param name="fileservice">The fileservice.</param>
    /// <param name="loggerFactory"></param>
    public DownloadFileService(
        IFileService<FileResult> fileservice,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DownloadFileService>();
        _logger.LogTrace($"DownloadFileService instance created with ID: {Guid.NewGuid()}");

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
#if WINDOWS
            var storageFile = await StorageFile.GetFileFromPathAsync(savedFile.FilePath);
            await Launcher.LaunchFileAsync(storageFile);
#endif
        }
    }
}
