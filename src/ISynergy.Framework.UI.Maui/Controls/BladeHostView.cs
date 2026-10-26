using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;

namespace ISynergy.Framework.UI.Controls;

/// <summary>
/// Shell-level host for the MAUI blade pane. It binds a <see cref="BladePane"/> to the shared
/// <see cref="BladeHostViewModel"/>, so blades opened from anywhere in the shell dock into the
/// same overlay pane instead of into the current page.
/// </summary>
/// <remarks>
/// The host resolves the view model lazily: the first attempt happens in the constructor and is
/// retried when the view is attached to a parent or loaded, because the platform service provider
/// may not be available yet during startup. When the view model cannot be resolved the view stays
/// collapsed and the failure is traced instead of thrown.
/// <para>
/// The host's own visibility follows the pane's: an empty pane is hidden, and a host that stayed
/// visible would span the whole shell and swallow every click aimed at the header, the sidebar or
/// the page underneath it.
/// </para>
/// </remarks>
/// <seealso cref="ContentView" />
public sealed class BladeHostView : ContentView
{
    private bool _resolved;

    /// <summary>
    /// Initializes a new instance of the <see cref="BladeHostView"/> class.
    /// </summary>
    public BladeHostView()
    {
        // Stay out of the hit-test path until the pane has something to show.
        IsVisible = false;

        Loaded += BladeHostView_Loaded;
        TryResolveHost();
    }

    /// <inheritdoc />
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (!_resolved)
            TryResolveHost();
    }

    private void BladeHostView_Loaded(object? sender, EventArgs e) => TryResolveHost();

    private void TryResolveHost()
    {
        if (_resolved)
            return;

        try
        {
            var host = IPlatformApplication.Current?.Services?.GetService<BladeHostViewModel>();

            if (host is null)
            {
                IsVisible = false;
                Trace.TraceError($"BladeHostView could not resolve {nameof(BladeHostViewModel)}; the blade pane stays hidden.");
                return;
            }

            BindingContext = host;

            var pane = new BladePane { ItemsSource = host.Blades };
            pane.PropertyChanged += OnPanePropertyChanged;
            Content = pane;

            IsVisible = pane.IsVisible;
            _resolved = true;
        }
        catch (Exception ex)
        {
            IsVisible = false;
            Trace.TraceError($"BladeHostView failed to attach the blade pane: {ex}");
        }
    }

    private void OnPanePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible) && sender is BladePane pane)
            IsVisible = pane.IsVisible;
    }
}
