using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

public interface IViewModelBladeView<TModel> : IViewModelBladeView
{
    AsyncRelayCommand<TModel> DeleteCommand { get; }
    AsyncRelayCommand<TModel> EditCommand { get; }
    ObservableCollection<TModel>? Items { get; set; }
    TModel? SelectedItem { get; set; }
    AsyncRelayCommand<TModel> SubmitCommand { get; }

    event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    Task EditAsync(TModel e);
    Task RemoveAsync(TModel e);
    void SetSelectedItem(TModel e, bool isUpdate = true);
    Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true);
}