using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;

namespace Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellView : View, IShellView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView" /> class.
        /// </summary>
        public ShellView()
        {
            InitializeComponent();
            ServiceLocator.Default.GetInstance<INavigationService>().Frame = ContentRootFrame;
        }
    }
}
