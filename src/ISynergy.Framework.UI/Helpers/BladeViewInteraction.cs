using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Head-neutral interaction rules shared by the WPF, WinUI and MAUI blade hosts.
/// </summary>
public static class BladeViewInteraction
{
    /// <summary>
    /// Closes the newest blade by executing its view model's close command, when it can execute.
    /// </summary>
    /// <param name="blades">The blades currently in effect, oldest first.</param>
    public static void CloseNewestBlade(IReadOnlyList<IView>? blades)
    {
        if (blades is null || blades.Count == 0)
            return;

        // Closing is delegated to the blade's view model: its Closed event drives
        // BaseBladeService.RemoveBlade, which removes the blade from the owner.
        var newest = blades[^1];

        if (newest.ViewModel is IViewModel viewModel && viewModel.CloseCommand.CanExecute(null))
            viewModel.CloseCommand.Execute(null);
    }
}
