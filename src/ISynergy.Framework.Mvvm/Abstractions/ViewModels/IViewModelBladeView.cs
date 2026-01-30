using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
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
    ObservableCollection<IView> Blades { get; set; }
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

public interface IViewModelBladeView<TModel> : IViewModelBladeView
{
    AsyncRelayCommand<TModel> DeleteCommand { get; }
    AsyncRelayCommand<TModel> EditCommand { get; }
    ObservableCollection<TModel> Items { get; set; }
    TModel? SelectedItem { get; set; }
    AsyncRelayCommand<TModel> SubmitCommand { get; }

    event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    Task EditAsync(TModel e);
    Task RemoveAsync(TModel e);
    void SetSelectedItem(TModel e, bool isUpdate = true);
    Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true);
}
