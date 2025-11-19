using ISynergy.Framework.Core.Utilities;
using Sample.ViewModels;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sample.Views.Identity.Controls;

/// <summary>
/// Class SignInControl. This class cannot be inherited.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
public sealed partial class SignInControl : UserControl
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>The view model.</value>
    private AuthenticationViewModel ViewModel => (DataContext as AuthenticationViewModel)!;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignInControl"/> class.
    /// </summary>
    public SignInControl()
    {
        InitializeComponent();

        Loaded += LoginScreen_Loaded;
        PreviewKeyDown += LoginScreen_PreviewKeyDown;
        TextBox_Password.GotFocus += TextBox_Password_GotFocus;
    }

    /// <summary>
    /// Handles the Unloaded event of the UserControl control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        Loaded -= LoginScreen_Loaded;
        PreviewKeyDown -= LoginScreen_PreviewKeyDown;
        TextBox_Password.GotFocus -= TextBox_Password_GotFocus;
    }

    /// <summary>
    /// Handles the GotFocus event of the TextBox_Password control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void TextBox_Password_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox_Password.SelectAll();
    }

    /// <summary>
    /// Handles the Loaded event of the LoginScreen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    private void LoginScreen_Loaded(object sender, RoutedEventArgs e)
    {
        SetCapsLockOnState();

        TextBox_Username.ItemsSource = ViewModel!.Usernames;

        if (TextBox_Username.Text.Length < 1)
        {
            TextBox_Username.Focus(FocusState.Keyboard);
        }
        else
        {
            TextBox_Password.Focus(FocusState.Keyboard);
        }
    }

    /// <summary>
    /// Handles the KeyDown event of the TextBox_Password control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
    private void TextBox_Password_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter && ViewModel.SignInCommand.CanExecute(null))
        {
            ViewModel.SignInCommand?.Execute(null);
        }
    }

    /// <summary>
    /// Handles the PreviewKeyDown event of the LoginScreen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
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

    /// <summary>
    /// Sets the state of the caps lock on.
    /// </summary>
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

    /// <summary>
    /// Texts the box username text changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="AutoSuggestBoxTextChangedEventArgs"/> instance containing the event data.</param>
    private void TextBox_Username_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                sender.ItemsSource = ViewModel!.Usernames;
            }
            else
            {
                sender.ItemsSource = ViewModel!.Usernames?.Where(q => q.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }

    /// <summary>
    /// Texts the box username query submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="AutoSuggestBoxQuerySubmittedEventArgs"/> instance containing the event data.</param>
    private void TextBox_Username_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
    }

    /// <summary>
    /// Texts the box username suggestion chosen.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="AutoSuggestBoxSuggestionChosenEventArgs"/> instance containing the event data.</param>
    private void TextBox_Username_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (NetworkUtility.IsValidEMail(args.SelectedItem.ToString()))
            sender.Text = args.SelectedItem.ToString();
    }
}
