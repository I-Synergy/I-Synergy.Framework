using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Accounts;
using Microsoft.UI.Xaml.Controls;
using Sample.ViewModels;

namespace Sample.Views.Identity.Controls;

/// <summary>
/// Class SignUpControl. This class cannot be inherited.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
public sealed partial class SignUpControl : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpControl"/> class.
    /// </summary>
    public SignUpControl()
    {
        InitializeComponent();
    }
}
