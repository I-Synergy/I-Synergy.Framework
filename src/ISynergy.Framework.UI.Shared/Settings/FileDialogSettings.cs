using System.Collections.Generic;

namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Settings for FileDialog />.
    /// </summary>
    public abstract class FileDialogSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box automatically adds an extension
        /// to a file name if the user omits the extension.
        /// </summary>
        /// <value><c>true</c> if [add extension]; otherwise, <c>false</c>.</value>
        public bool AddExtension { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays a warning if the user
        /// specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if [check file exists]; otherwise, <c>false</c>.</value>
        public bool CheckFileExists { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays a warning if the user
        /// specifies a path that does not exist.
        /// </summary>
        /// <value><c>true</c> if [check path exists]; otherwise, <c>false</c>.</value>
        public bool CheckPathExists { get; set; } = true;

        /// <summary>
        /// Gets or sets the default file name extension.
        /// </summary>
        /// <value>The default ext.</value>
        public string DefaultExt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a string containing the file name selected in the file dialog box.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the file names of all selected files in the dialog box.
        /// </summary>
        /// <value>The file names.</value>
        public List<string> FileNames { get; set; } = new List<string> { string.Empty };

        /// <summary>
        /// Gets or sets the current file name filter string, which determines the choices that
        /// appear in the "Save as file type" or "Files of type" box in the dialog box.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the filter.</value>
        public int FilterIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets the initial directory displayed by the file dialog box.
        /// </summary>
        /// <value>The initial directory.</value>
        public string InitialDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file dialog box title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; } = string.Empty;
    }
}
