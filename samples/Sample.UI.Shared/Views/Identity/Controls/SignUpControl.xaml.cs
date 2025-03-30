using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Accounts;
using Microsoft.UI.Xaml.Controls;
using Sample.ViewModels;
using Syncfusion.UI.Xaml.Editors;

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

    private void ComboBox_Modules_SelectionChanged(object sender, ComboBoxSelectionChangedEventArgs e)
    {
        if (DataContext is AuthenticationViewModel viewModel)
        {
            foreach (Module item in e.AddedItems.EnsureNotNull())
                viewModel.Registration_Modules.Add(item);

            foreach (Module item in e.RemovedItems.EnsureNotNull())
                viewModel.Registration_Modules.Remove(item);
        }
    }

    private void ComboBox_Modules_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (DataContext is AuthenticationViewModel viewModel && viewModel.Modules.FirstOrDefault() is { } module)
        {
            ComboBox_Modules.SelectedItems.Add(module);
        }

    }
}
