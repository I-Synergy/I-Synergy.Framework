using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.Controls;

namespace Sample.Views;

/// <summary>
/// Class ForgotPasswordWindow. This class cannot be inherited.
/// Implements the <see cref="ISynergy.Framework.UI.Controls.Window" />
/// Implements the <see cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
/// Implements the <see cref="IForgotPasswordWindow" />
/// </summary>
/// <seealso cref="ISynergy.Framework.UI.Controls.Window" />
/// <seealso cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
/// <seealso cref="IForgotPasswordWindow" />
public sealed partial class ForgotPasswordWindow : Window, IForgotPasswordWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordWindow"/> class.
    /// </summary>
    public ForgotPasswordWindow()
    {
        InitializeComponent();
        PrimaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Ok");
        SecondaryButtonText = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("Cancel");
    }
}
