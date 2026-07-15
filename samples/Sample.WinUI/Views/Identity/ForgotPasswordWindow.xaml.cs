using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

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
        var languageService = Application.Services.GetRequiredService<ILanguageService>();
        PrimaryButtonText = languageService.GetString("Ok");
        SecondaryButtonText = languageService.GetString("Cancel");
    }
}
