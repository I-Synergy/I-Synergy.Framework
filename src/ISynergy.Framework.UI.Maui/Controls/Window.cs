using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using Mopups.Pages;

namespace ISynergy.Framework.UI.Controls
{
    public class Window : PopupPage, IWindow
    {
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(nameof(ViewModel), typeof(IViewModel), typeof(Window), null);

        public IViewModel ViewModel
        {
            get => (IViewModel)GetValue(ViewModelProperty);
            set
            {
                SetValue(ViewModelProperty, value);
                BindingContext = value;
            } 
        }

        public Window()
        {
            CloseWhenBackgroundIsClicked = false;
            BackgroundColor = Color.FromArgb("#80000000");
            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, EventArgs e)
        {
            Loaded -= Window_Loaded;

            if (ViewModel is not null)
                await ViewModel.InitializeAsync();
        }
    }
}
