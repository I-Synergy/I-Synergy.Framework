using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions;

/// <summary>
/// Interface IWindow
/// </summary>
public interface IWindow : IDisposable
{
    /// <summary>
    /// Gets or sets the viewmodel.
    /// </summary>
    /// <value>The data context.</value>
    IViewModel ViewModel { get; set; }

    /// <summary>
    /// Closes the current window.
    /// </summary>
    Task CloseAsync();
}
