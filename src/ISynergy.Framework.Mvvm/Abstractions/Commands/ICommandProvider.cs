using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions.Commands;

/// <summary>
/// Provides a compile-time-safe way to expose the set of <see cref="ICommand"/> instances
/// available on a ViewModel, as an alternative to runtime property reflection.
/// </summary>
/// <remarks>
/// Implement this interface on ViewModels to allow AOT-compatible command discovery.
/// <para>
/// When a ViewModel implements <see cref="ICommandProvider"/>, platform services such as
/// <c>NavigationService.HasRunningCommands</c> and <c>View.SubscribeToViewModelCommands</c>
/// will use <see cref="GetCommands"/> instead of <c>GetProperties</c>-based reflection,
/// which is incompatible with NativeAOT and the .NET trimmer.
/// </para>
/// <example>
/// <code>
/// public class MyViewModel : ViewModelBase, ICommandProvider
/// {
///     public IAsyncRelayCommand SaveCommand { get; } = new AsyncRelayCommand(SaveAsync);
///
///     IEnumerable&lt;ICommand&gt; ICommandProvider.GetCommands()
///         => [SaveCommand];
/// }
/// </code>
/// </example>
/// </remarks>
public interface ICommandProvider
{
    /// <summary>
    /// Returns all <see cref="ICommand"/> instances exposed by this ViewModel.
    /// </summary>
    /// <returns>
    /// An enumerable of <see cref="ICommand"/> instances that should participate
    /// in command-discovery scenarios (e.g., <c>CanExecuteChanged</c> subscription,
    /// running-command detection during navigation).
    /// </returns>
    IEnumerable<ICommand> GetCommands();
}
