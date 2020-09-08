using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Dialogs
{
    /// <summary>
    /// Settings for WinMessageBox />.
    /// </summary>
    public class MessageBoxSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="string" /> that specifies the text to display.
        /// </summary>
        /// <value>The message box text.</value>
        public string MessageBoxText { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="string" /> that specifies the title bar caption to display.
        /// Default value is an empty string.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; set; } = "";

        /// <summary>
        /// Gets or sets the <see cref="MessageBoxButton" /> value that specifies which button or
        /// buttons to display. Default value is <see cref="MessageBoxButton.OK" />.
        /// </summary>
        /// <value>The button.</value>
        public MessageBoxButton Button { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MessageBoxImage" /> value that specifies the icon to
        /// display. Default value is <see cref="MessageBoxImage.None" />.
        /// </summary>
        /// <value>The icon.</value>
        public MessageBoxImage Icon { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MessageBoxResult" /> value that specifies the default result
        /// of the message box. Default value is <see cref="MessageBoxResult.None" />.
        /// </summary>
        /// <value>The default result.</value>
        public MessageBoxResult DefaultResult { get; set; }
    }
}
