namespace ISynergy.Framework.AspNetCore.ViewModels.Base;

/// <summary>
/// Interface IBaseViewModel
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IBaseViewModel : IDisposable
{
    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    Task InitializeAsync();
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    string Title { get; }
}
