namespace ISynergy.Framework.Windows.Dialogs
{
    /// <summary>
    /// Settings for <see cref="OpenFileDialog"/>.
    /// </summary>
    public class OpenFileDialogSettings : FileDialogSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows multiple files to be
        /// selected.
        /// </summary>
        public bool Multiselect { get; set; }
    }
}
