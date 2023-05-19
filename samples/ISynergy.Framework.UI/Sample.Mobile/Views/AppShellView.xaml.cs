using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;

namespace Sample.Views
{
    public partial class AppShellView : Shell, IShellView
    {
        public IViewModel ViewModel
        {
            get => BindingContext is IViewModel viewModel ? viewModel : null;
            set => BindingContext = value;
        }

        public AppShellView()
        {
            InitializeComponent();
        }
    }
}