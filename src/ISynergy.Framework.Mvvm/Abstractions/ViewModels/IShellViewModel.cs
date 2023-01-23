using CommunityToolkit.Mvvm.Input;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IShellViewModel
    /// Implements the <see cref="IViewModel" />
    /// </summary>
    /// <seealso cref="IViewModel" />
    public interface IShellViewModel : IViewModel
    {
        /// <summary>
        /// Gets or sets the settings command.
        /// </summary>
        /// <value>The settings command.</value>
        AsyncRelayCommand Settings_Command { get; set; }
    }
}
