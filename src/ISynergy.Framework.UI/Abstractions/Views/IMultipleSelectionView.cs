using ISynergy.Framework.Mvvm.Abstractions;

namespace ISynergy.Framework.UI.Abstractions.Views;

/// <summary>
/// Marker interface for a multiple-selection view.
/// Resolved by the navigation service for ViewModels implementing <see cref="ISynergy.Framework.Mvvm.Abstractions.ViewModels.IMultipleSelectionViewModel{TModel}"/>.
/// </summary>
public interface IMultipleSelectionView : IView
{
}
