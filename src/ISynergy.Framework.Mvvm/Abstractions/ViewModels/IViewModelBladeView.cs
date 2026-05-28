using ISynergy.Framework.Mvvm.Commands;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelBladeView
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IViewModelBladeView : IViewModel
{
    AsyncRelayCommand AddCommand { get; }
    ObservableCollection<IView>? Blades { get; set; }
    bool DisableRefreshOnInitialization { get; set; }
    bool IsPaneVisible { get; set; }
    bool IsUpdate { get; set; }
    AsyncRelayCommand RefreshCommand { get; }
    AsyncRelayCommand<object> SearchCommand { get; }

    Task AddAsync();
    Task RefreshAsync(CancellationToken cancellationToken = default);
    Task RetrieveItemsAsync(CancellationToken cancellationToken);
    Task SearchAsync(object e);
}