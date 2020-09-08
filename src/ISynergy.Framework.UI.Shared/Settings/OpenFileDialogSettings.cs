namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Settings for OpenFileDialog />.
    /// </summary>
    public class OpenFileDialogSettings : FileDialogSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows multiple files to be
        /// selected.
        /// </summary>
        /// <value><c>true</c> if multiselect; otherwise, <c>false</c>.</value>
        public bool Multiselect { get; set; }
    }
}
