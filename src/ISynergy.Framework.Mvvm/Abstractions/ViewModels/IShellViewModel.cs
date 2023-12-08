using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IShellViewModel
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IShellViewModel : IViewModel
{
    /// <summary>
    /// Gets or sets the settings _command.
    /// </summary>
    /// <value>The settings _command.</value>
    AsyncRelayCommand SettingsCommand { get; }
}
