using ISynergy.Handlers;
using ISynergy.ViewModels.Authentication;
using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ISynergy.Views.Authentication.Controls
{
    public sealed partial class LoginControl : UserControl
    {
        private ILoginViewModel ViewModel => DataContext as ILoginViewModel;

        public LoginControl()
        {
            InitializeComponent();

            Loaded += LoginScreen_Loaded;
            PreviewKeyDown += LoginScreen_PreviewKeyDown;
            TextBox_Password.GotFocus += TextBox_Password_GotFocus;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= LoginScreen_Loaded;
            PreviewKeyDown -= LoginScreen_PreviewKeyDown;
            TextBox_Password.GotFocus -= TextBox_Password_GotFocus;
        }

        private void TextBox_Password_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox_Password.SelectAll();
        }

        private void LoginScreen_Loaded(object sender, RoutedEventArgs e)
        {
            SetCapsLockOnState();

            TextBox_Username.ItemsSource = ViewModel.Usernames;

            if (TextBox_Username.Text.Length < 1)
            {
                TextBox_Username.Focus(FocusState.Keyboard);
            }
            else
            {
                TextBox_Password.Focus(FocusState.Keyboard);
            }
        }

        private void TextBox_Password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && ViewModel.Submit_Command.CanExecute(null))
            {
                ViewModel.Submit_Command.Execute(null);
            }
        }

        private void LoginScreen_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            SetCapsLockOnState();

            switch (e.Key)
            {
                case VirtualKey.Enter:
                    //UIElement FocusedElement = Keyboard.FocusedElement as UIElement;

                    //if ((FocusedElement is null) == false)
                    //{
                    //    if (FocusedElement.Uid == "Button_Submit" | FocusedElement.Uid == "TextBox_Password")
                    //    {
                    //        if (Button_Submit.Command.CanExecute(null) == true)
                    //        {
                    //            Button_Submit.Command.Execute(null);
                    //        }

                    //        e.Handled = true;
                    //    }
                    //    else
                    //    {
                    //        FocusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    //        e.Handled = true;
                    //    }
                    //}
                    break;
                case VirtualKey.Escape:
                    //this.Close();
                    e.Handled = true;
                    break;
                default:
                    e.Handled = false;
                    break;
            }
        }

        private void SetCapsLockOnState()
        {
            if (Console.CapsLock)
            {
                TextBox_CapsLockOn.Visibility = Visibility.Visible;
            }
            else
            {
                TextBox_CapsLockOn.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_Username_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if(string.IsNullOrWhiteSpace(sender.Text))
                {
                    sender.ItemsSource = ViewModel.Usernames;
                }
                else
                {
                    sender.ItemsSource = ViewModel.Usernames?.Where(q => q.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
        }

        private static void TextBox_Username_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
        }

        private static void TextBox_Username_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if(Network.IsValidEMail(args.SelectedItem.ToString()))
            {
                sender.Text = args.SelectedItem.ToString();
            }
        }
    }
}
