using ISynergy.Framework.Mvvm.Abstractions;

namespace ISynergy.Framework.UI.Abstractions.Views;

/// <summary>
/// Marker interface for a single-selection view.
/// Resolved by the navigation service for ViewModels implementing <see cref="ISynergy.Framework.Mvvm.Abstractions.ViewModels.ISingleSelectionViewModel"/>.
/// </summary>
public interface ISingleSelectionView : IView
{
}
