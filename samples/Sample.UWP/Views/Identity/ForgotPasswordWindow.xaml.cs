using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.Controls;

namespace Sample.Views;

/// <summary>
/// Class ForgotPasswordWindow. This class cannot be inherited.
/// Implements the <see cref="ISynergy.Framework.UI.Controls.Window" />
/// Implements the <see cref="Windows.UI.Xaml.Markup.IComponentConnector" />
/// Implements the <see cref="IForgotPasswordWindow" />
/// </summary>
/// <seealso cref="ISynergy.Framework.UI.Controls.Window" />
/// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
/// <seealso cref="IForgotPasswordWindow" />
public sealed partial class ForgotPasswordWindow : Window, IForgotPasswordWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordWindow"/> class.
    /// </summary>
    public ForgotPasswordWindow()
    {
        InitializeComponent();
        PrimaryButtonText = LanguageService.Default.GetString("Ok");
        SecondaryButtonText = LanguageService.Default.GetString("Cancel");
    }
}
