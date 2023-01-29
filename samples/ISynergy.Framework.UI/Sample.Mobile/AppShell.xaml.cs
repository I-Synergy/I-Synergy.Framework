using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace Sample
{
    public partial class AppShell : Shell
    {
        public AppShell(IShellViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}