using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;

namespace Sample
{
    public partial class AppShell : Shell, IShellView
    {
        public IViewModel ViewModel { get; set; }

        public AppShell(IShellViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}