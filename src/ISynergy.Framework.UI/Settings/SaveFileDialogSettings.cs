namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Settings for SaveFileDialog />.
    /// </summary>
    public class SaveFileDialogSettings : FileDialogSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box prompts the user for permission
        /// to create a file if the user specifies a file that does not exist.
        /// </summary>
        /// <value><c>true</c> if [create prompt]; otherwise, <c>false</c>.</value>
        public bool CreatePrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <b>Save As</b> dialog box displays a
        /// warning if the user specifies a file name that already exists.
        /// </summary>
        /// <value><c>true</c> if [overwrite prompt]; otherwise, <c>false</c>.</value>
        public bool OverwritePrompt { get; set; }
    }
}
