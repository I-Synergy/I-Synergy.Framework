using Sample.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sample.Views.Controls;
/// <summary>
/// Interaction logic for SignInControl.xaml
/// </summary>
public partial class SignInControl : UserControl
{
    public SignInControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the KeyDown event of the TextBox_Password control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    private void TextBox_Password_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is AuthenticationViewModel viewModel)
            viewModel.SignInCommand?.Execute(null);
    }
}
