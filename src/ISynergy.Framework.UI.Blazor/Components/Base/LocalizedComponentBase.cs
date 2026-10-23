using ISynergy.Framework.Globalization.Services;
using Microsoft.AspNetCore.Components;

namespace ISynergy.Framework.UI.Components.Base;

/// <summary>
/// Base class for Blazor components that respond to language changes at runtime.
/// </summary>
/// <remarks>
/// Components that inherit from this class will automatically re-render when the application
/// language is changed via <see cref="ISynergy.Framework.Globalization.Services.LocalizedStrings.Refresh"/>.
/// This enables dynamic language switching without page reload.
/// </remarks>
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    /// <summary>
    /// Called when the component is initialized.
    /// Subscribes to localization change notifications.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LocalizedStrings.Default.Changed += OnLocalizationChanged;
    }

    /// <summary>
    /// Handles localization change events by triggering a component re-render.
    /// </summary>
    private void OnLocalizationChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Releases resources used by the component.
    /// Unsubscribes from localization change notifications.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            LocalizedStrings.Default.Changed -= OnLocalizationChanged;
        }
    }
}
