﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Base class for file services.
/// </summary>
public class FileService : IFileService<FileResult>
{
    /// <summary>
    /// The dialog service.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// The language service.
    /// </summary>
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService" /> class.
    /// </summary>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="languageService">The language service.</param>
    public FileService(IDialogService dialogService, ILanguageService languageService)
    {
        _dialogService = dialogService;
        _languageService = languageService;
    }

    /// <summary>
    /// Determines the filename of the file what will be used.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="filename">The filename.</param>
    /// <param name="file">The file.</param>
    /// <returns>FileResult.</returns>
    public async Task<FileResult> SaveFileAsync(string folder, string filename, byte[] file)
    {
        var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
        var createdFile = await storageFolder.CreateFileAsync(
                filename,
                CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(createdFile, file);

        return new FileResult(
                createdFile.Path,
                createdFile.Name,
                () => createdFile.OpenStreamForReadAsync().GetAwaiter().GetResult());
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
                await _dialogService.ShowErrorAsync(string.Format(_languageService.GetString("WarningDocumentSizeTooBig"), $"{maxFileSize} bytes"));
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
            var splittedFilters = filter.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
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
    public async Task<byte[]> BrowseImageAsync(string[] filter, long maxFileSize = 1 * 1024 * 1024)
    {
        if (await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is List<FileResult> result)
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
    private async Task<List<FileResult>> PickFileAsync(string[] allowedTypes = null, bool multiple = false)
    {
        var result = new List<FileResult>();

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        };

#if WINDOWS10_0_17763_0_OR_GREATER
        var _mainWindow = ((BaseApplication)Application.Current)?.MainWindow;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
#endif

        if (allowedTypes is not null)
        {
            var hasAtleastOneType = false;

            foreach (var type in allowedTypes.Where(q => q.StartsWith(".")))
            {
                picker.FileTypeFilter.Add(type);
                hasAtleastOneType = true;
            }

            if (!hasAtleastOneType)
            {
                picker.FileTypeFilter.Add("*");
            }
        }
        else
        {
            picker.FileTypeFilter.Add("*");
        }

        if (multiple)
        {
            if (await picker.PickMultipleFilesAsync() is IReadOnlyList<StorageFile> files)
            {
                foreach (var file in files)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(file);

                    result.Add(new FileResult(
                        file.Path,
                        file.Name,
                        () => file.OpenStreamForReadAsync().GetAwaiter().GetResult()));
                }
            }
        }
        else
        {
            if (await picker.PickSingleFileAsync() is StorageFile file)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);

                result.Add(new FileResult(
                    file.Path,
                    file.Name,
                    () => file.OpenStreamForReadAsync().GetAwaiter().GetResult()));
            }
        }

        return result;
    }

    /// <summary>
    /// UWP implementation of OpenFile(), opening a file already stored in the app's local
    /// folder directory.
    /// storage.
    /// </summary>
    /// <param name="fileToOpen">relative filename of file to open</param>
    public async Task OpenFileAsync(string fileToOpen)
    {
        try
        {
            if (await ApplicationData.Current.LocalFolder.GetFileAsync(fileToOpen) is StorageFile file)
                await Launcher.LaunchFileAsync(file);
        }
        catch (FileNotFoundException)
        {
            // ignore exceptions
        }
        catch (Exception)
        {
            // ignore exceptions
        }
    }
}
