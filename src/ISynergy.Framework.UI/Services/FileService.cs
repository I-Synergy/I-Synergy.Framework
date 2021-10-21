using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.IO;
using Windows.System;
using System.Linq;

#if WINDOWS
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Xaml;
#endif

namespace ISynergy.Framework.UI.Services
{
#if WINDOWS
    /// <summary>
    /// IInitializeWithWindow interface.
    /// </summary>
    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        /// <summary>
        /// Initialize with window.
        /// </summary>
        /// <param name="hwnd"></param>
        void Initialize(IntPtr hwnd);
    }

    /// <summary>
    /// IWindowNative interface.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    internal interface IWindowNative
    {
        /// <summary>
        /// Get window handle.
        /// </summary>
        IntPtr WindowHandle { get; }
    }
#endif

    /// <summary>
    /// Base class for file services.
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        /// The dialog service
        /// </summary>
        private readonly IDialogService _dialogService;
        /// <summary>
        /// The language service
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService" /> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="languageService">The language service.</param>
        public FileService(
            IDialogService dialogService,
            ILanguageService languageService)
        {
            _dialogService = dialogService;
            _languageService = languageService;
            
            AddExtension = true;
            CheckFileExists = false;
            CheckPathExists = true;
            FilterIndex = 1;
            ValidateNames = true;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

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
        /// <remarks>If this method returns <c>true</c>, the <see cref="FileName" /> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.</remarks>
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
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>FileResult.</returns>
        public async Task<FileResult> BrowseFileAsync(string filefilter, long maxfilesize)
        {
            var filters = GetFilters(filefilter);

            if (await PickFileAsync(filters.ToArray()) is FileResult file)
            {
                if (file.File.Length <= maxfilesize || maxfilesize == 0)
                {
                    return file;
                }
                else
                {
                    await _dialogService.ShowErrorAsync(string.Format(_languageService.GetString("WarningDocumentSizeTooBig"), $"{maxfilesize / (1024 * 1024)}MB"));
                }
            }

            return null;
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
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>System.Byte[].</returns>
        public async Task<byte[]> BrowseImageAsync(string[] filter, long maxfilesize = 0)
        {
            if(await BrowseFileAsync(string.Join(";", filter), maxfilesize) is FileResult result)
            {
                return result.File;
            }

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
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        private static async Task<FileResult> PickFileAsync(string[] allowedTypes = null)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

#if WINDOWS
            var hwnd = Window.Current.CoreWindow.As<IWindowNative>().WindowHandle;
            var initializeWithWindow = picker.As<IInitializeWithWindow>();
            initializeWithWindow.Initialize(hwnd);
#endif

            if (allowedTypes != null)
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

            if (await picker.PickSingleFileAsync() is StorageFile file)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);

                return new FileResult(
                    file.Path,
                    file.Name,
                    () => file.OpenStreamForReadAsync().GetAwaiter().GetResult());
            }

            return null;
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
