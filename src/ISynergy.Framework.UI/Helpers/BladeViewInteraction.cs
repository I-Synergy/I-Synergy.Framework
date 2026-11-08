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

    /// <summary>
    /// Releases and removes the hosted cards whose blade is no longer in the wanted set.
    /// </summary>
    /// <typeparam name="TCard">The head-specific card type.</typeparam>
    /// <param name="wanted">The blades that should be rendered, in order.</param>
    /// <param name="cards">The cards hosted by the control, keyed by blade.</param>
    /// <param name="release">Releases one card before it is dropped.</param>
    public static void RemoveStaleCards<TCard>(
        IReadOnlyList<IView> wanted,
        IDictionary<IView, TCard> cards,
        Action<TCard> release)
    {
        foreach (var stale in cards.Keys.Where(blade => !wanted.Contains(blade)).ToList())
        {
            release(cards[stale]);
            cards.Remove(stale);
        }
    }
}
