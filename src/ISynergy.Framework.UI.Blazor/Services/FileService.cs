using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Base class for file services.
/// </summary>
public class FileService : IFileService<FileResult>
{
    private readonly ILogger _logger;
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService" /> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="logger"></param>
    public FileService(
        IDialogService dialogService,
        ILogger<FileService> logger)
    {
        _logger = logger;
        _logger.LogTrace($"FileService instance created with ID: {Guid.NewGuid()}");

        _dialogService = dialogService;
    }

    /// <summary>
    /// Determines the filename of the file what will be used.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="file">The file.</param>
    /// <returns>FileResult.</returns>
    public async Task<FileResult?> SaveFileAsync(string folder, string filename, byte[] file)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// browse file as an asynchronous operation.
    /// </summary>
    /// <param name="filefilter">The filefilter.</param>
    /// <param name="multiple"></param>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>FileResult.</returns>
    public async Task<List<FileResult>> BrowseFileAsync(string filefilter, bool multiple = false, long maxFileSize = 1 * 1024 * 1024)
    {
        var result = new List<FileResult>();
        var filters = GetFilters(filefilter);

        foreach (var file in await PickFileAsync(filters.ToArray(), multiple))
        {
            if (file.File.Length <= maxFileSize || maxFileSize == 0)
            {
                result.Add(file);
            }
            else
            {
                await _dialogService.ShowErrorAsync(string.Format(LanguageService.Default.GetString("WarningDocumentSizeTooBig"), $"{maxFileSize} bytes"));
            }
        }

        return result;
    }

    private List<string> GetFilters(string filter)
    {
        var result = new List<string>();
        var fileFilter = string.Empty;

        // Support full .NET filters (like "Text files|*.txt") as well
        if (filter.Contains("|"))
        {
            var splittedFilters = filter.Split(["|"], StringSplitOptions.RemoveEmptyEntries);
            if (splittedFilters.Length == 2)
            {
                fileFilter = splittedFilters[1].Trim();
            }
        }

        if (!string.IsNullOrWhiteSpace(fileFilter))
        {
            var array = fileFilter.Split(';');

            foreach (var item in array.EnsureNotNull())
            {
                if (item.Trim().StartsWith("*") && !item.Trim().Equals("*.*"))
                {
                    result.Add(item.Trim().Replace("*", string.Empty));
                }
                else
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// get image as an asynchronous operation.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="maxFileSize">Maximum filesize, default 1Mb (1 * 1024 * 1024)</param>
    /// <returns>System.Byte[].</returns>
    public async Task<byte[]?> BrowseImageAsync(string[] filter, long maxFileSize = 1 * 1024 * 1024)
    {
        if (await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is { } result)
            return result.First().File;

        return null;
    }

    /// <summary>
    /// Implementation for picking a file on UWP platform.
    /// </summary>
    /// <param name="allowedTypes">
    /// Specifies one or multiple allowed types. When null, all file types
    /// can be selected while picking.
    /// On UWP, specify a list of extensions, like this: ".jpg", ".png".
    /// </param>
    /// <param name="multiple"></param>
    /// <returns>
    /// File data object, or null when user cancelled picking file
    /// </returns>
    private async Task<List<FileResult>> PickFileAsync(string[]? allowedTypes = null, bool multiple = false)
    {
        throw new NotImplementedException();
    }

    public Task OpenFileAsync(string fileToOpen) => throw new NotImplementedException();
}
