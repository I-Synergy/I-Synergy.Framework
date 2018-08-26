using CommonServiceLocator;
using ISynergy.Services;
using ISynergy.ViewModels.Authentication;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Views.Authentication
{
    public sealed partial class TagWindow : ITagWindow
    {
        private TagViewModel ViewModel => DataContext as TagViewModel;

        public TagWindow()
        {
            this.InitializeComponent();
            this.Loaded += TagWindow_Loaded;
            this.DataContextChanged += TagWindow_DataContextChanged;
        }

        private void TagWindow_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if(args.NewValue != null && args.NewValue is TagViewModel)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsValid))
            {
                if (ViewModel.IsValid)
                {
                    TagIcon.Fill = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    TagIcon.Fill = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void TagWindow_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PasswordBox_Tag.Focus(FocusState.Keyboard);
        }

        private void PasswordBox_Tag_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && ViewModel.Submit_Command.CanExecute(null))
            {
                ViewModel.Submit_Command.Execute(null);
            }
        }
    }
}
