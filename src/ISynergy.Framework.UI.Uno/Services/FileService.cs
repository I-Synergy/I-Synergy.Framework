﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System;

namespace ISynergy.Framework.UI.Services
{
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
        /// Main window reference.
        /// </summary>
        private readonly Microsoft.UI.Xaml.Window _mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="languageService">The language service.</param>
        public FileService(IDialogService dialogService, ILanguageService languageService)
        {
            _mainWindow = ((BaseApplication)Application.Current)?.MainWindow;
            _dialogService = dialogService;
            _languageService = languageService;
            
            AddExtension = true;
            CheckFileExists = false;
            CheckPathExists = true;
            FilterIndex = 1;
            ValidateNames = true;
        }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value><c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        public bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the selected filter. The default is <c>1</c>.</value>
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; private set; }
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool ValidateNames { get; set; }

        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="file">The file.</param>
        /// <returns>FileResult.</returns>
        public async Task<FileResult> SaveFileAsync(string filename, byte[] file)
        {
            var createdFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
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
            if(await BrowseFileAsync(string.Join(";", filter), false, maxFileSize) is List<FileResult> result)
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

            if(multiple)
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
}
