using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Shell-level host for the framework blade pane. It binds a <see cref="BladeView"/> to the
/// shared <see cref="BladeHostViewModel"/>, so blades opened from anywhere in the shell dock
/// into the same pane instead of into the current page.
/// </summary>
/// <seealso cref="UserControl" />
public partial class BladeHostView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BladeHostView"/> class.
    /// </summary>
    /// <remarks>
    /// When the host view model is not resolvable (design time, or a host that does not register
    /// it) the control stays collapsed and traces the failure instead of throwing.
    /// </remarks>
    public BladeHostView()
    {
        InitializeComponent();

        try
        {
            var host = Application.Services.GetRequiredService<BladeHostViewModel>();
            DataContext = host;
        }
        catch (Exception ex)
        {
            Visibility = Visibility.Collapsed;
            Trace.TraceError($"BladeHostView could not resolve {nameof(BladeHostViewModel)}: {ex}");
        }
    }
}
