using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;

namespace Sample
{
    public partial class AppShell : Shell, IShellView
    {
        public IViewModel ViewModel { get; set; }

        public AppShell(IContext context)
        {
            InitializeComponent();
            ViewModel = context.ScopedServices.ServiceProvider.GetRequiredService<IShellViewModel>() as IViewModel;
            BindingContext = ViewModel;
        }
    }
}