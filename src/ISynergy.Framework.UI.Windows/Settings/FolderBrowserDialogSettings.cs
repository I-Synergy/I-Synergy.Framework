namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Settings for FolderBrowserDialog />.
    /// </summary>
    public class FolderBrowserDialogSettings
    {
        /// <summary>
        /// Gets or sets the descriptive text displayed above the tree view control in the dialog
        /// box.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the path selected by the user.
        /// </summary>
        /// <value>The selected path.</value>
        public string SelectedPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the New Folder button appears in the folder
        /// browser dialog box.
        /// </summary>
        /// <value><c>true</c> if [show new folder button]; otherwise, <c>false</c>.</value>
        public bool ShowNewFolderButton { get; set; }
    }
}
