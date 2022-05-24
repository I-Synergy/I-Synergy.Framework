using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

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
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool ValidateNames { get; set; }

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
        /// Configures the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        private void ConfigureFileDialog(FileDialog fileDialog)
        {
            Argument.IsNotNull(fileDialog);

            string initialDirectory = string.Empty;

            fileDialog.Filter = Filter;
            fileDialog.FileName = FileName;
            fileDialog.AddExtension = AddExtension;
            fileDialog.CheckFileExists = CheckFileExists;
            fileDialog.CheckPathExists = CheckPathExists;
            fileDialog.FilterIndex = FilterIndex;
            fileDialog.InitialDirectory = Path.Combine(initialDirectory);
            fileDialog.Title = Title;
            fileDialog.ValidateNames = ValidateNames;
        }

        public Task<FileResult> SaveFileAsync(string filename, byte[] file)
        {
            throw new System.NotImplementedException();
        }

        public Task<FileResult> BrowseFileAsync(string filefilter, long maxfilesize)
        {
            throw new System.NotImplementedException();
        }

        public Task<byte[]> BrowseImageAsync(string[] filter, long maxfilesize = 0)
        {
            throw new System.NotImplementedException();
        }

        public Task OpenFileAsync(string fileToOpen)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the file names in case <see cref="IsMultiSelect"/> is <c>true</c>.
        /// </summary>
        /// <remarks></remarks>
        public string[] FileNames { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        public bool IsMultiSelect { get; set; }

        public virtual Task<bool> DetermineFileAsync()
        {
            var fileDialog = new OpenFileDialog();
            ConfigureFileDialog(fileDialog);

            fileDialog.Multiselect = IsMultiSelect;

            bool result = fileDialog.ShowDialog() ?? false;

            if (result)
            {
                FileName = fileDialog.FileName;
                FileNames = fileDialog.FileNames;
            }
            else
            {
                FileName = null;
                FileNames = null;
            }

            return Task.FromResult(result);
        }
    }
}
