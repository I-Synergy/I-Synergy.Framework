using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.ViewModels;

namespace ISynergy.Framework.UI.Sample.Views.Library
{
    /// <summary>
    /// Class NoteWindow. This class cannot be inherited.
    /// </summary>
    public sealed partial class NoteWindow : INoteWindow
    {
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        private NoteViewModel ViewModel => DataContext as NoteViewModel;

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
