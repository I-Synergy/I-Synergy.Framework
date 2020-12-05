using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Controls;

namespace ISynergy.Framework.UI.Services
{
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
        public Task<FileResult> SaveFileAsync(string filename, byte[] file) =>
            FilePicker.Current.SaveFileAsync(filename, file);

        /// <summary>
        /// browse file as an asynchronous operation.
        /// </summary>
        /// <param name="filefilter">The filefilter.</param>
        /// <param name="maxfilesize">The maxfilesize.</param>
        /// <returns>FileResult.</returns>
        public async Task<FileResult> BrowseFileAsync(string filefilter, long maxfilesize)
        {
            var filters = new List<string>();
            var filterArray = filefilter?.Split(';');

            foreach (var filter in filterArray.EnsureNotNull())
            {
                filters.AddRange(GetFilters(filter));
            }

            if (await FilePicker.Current.PickFileAsync(filters.ToArray()) is FileResult file)
            {
                if (file.File.Length <= maxfilesize || maxfilesize == 0)
                {
                    return file;
                }
                else
                {
                    await _dialogService.ShowErrorAsync(string.Format(_languageService.GetString("Warning_Document_SizeTooBig"), $"{maxfilesize / (1024 * 1024)}MB"));
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
                if (filter.Trim().StartsWith("*") && !filter.Trim().Equals("*.*"))
                {
                    fileFilter = filter.Trim().Replace("*", string.Empty);
                }

                result.Add(fileFilter);
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
    }
}
