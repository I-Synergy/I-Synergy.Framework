namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class NoteWindow. This class cannot be inherited.
    /// </summary>

    /* Unmerged change from project 'ISynergy.Framework.UI (net6.0-windows10.0.22000.0)'
    Before:
        public sealed partial class NoteWindow : ISynergy.Framework.UI.Controls.Window, INoteWindow
    After:
        public sealed partial class NoteWindow : Controls.Windows.Window, INoteWindow
    */

    /* Unmerged change from project 'ISynergy.Framework.UI (net6.0-windows)'
    Before:
        public sealed partial class NoteWindow : ISynergy.Framework.UI.Controls.Window, INoteWindow
    After:
        public sealed partial class NoteWindow : Controls.Windows.Window, INoteWindow
    */
    public sealed partial class NoteWindow : Window, INoteWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteWindow"/> class.
        /// </summary>
        public NoteWindow()
        {
            InitializeComponent();

            PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Ok");
            SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Close");
        }
    }
}
