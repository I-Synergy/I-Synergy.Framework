using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Sample.ViewModels;
using Windows.System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sample.Views.Identity.Controls;

/// <summary>
/// Class SignInControl. This class cannot be inherited.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
public sealed partial class SignInControl : UserControl
{
    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>The view model.</value>
    private AuthenticationViewModel? ViewModel => DataContext as AuthenticationViewModel;

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
        if (e.Key == VirtualKey.Enter && ViewModel?.SignInCommand.CanExecute(null) == true)
        {
            ViewModel?.SignInCommand?.Execute(null);
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
}
