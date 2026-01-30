using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelBlade
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IViewModelBlade : IViewModel
{
    bool IsDisabled { get; set; }
    bool IsUpdate { get; set; }
    IViewModelBladeView Owner { get; set; }
}

public interface IViewModelBlade<TModel> : IViewModelBlade
{
    TModel? SelectedItem { get; set; }
    AsyncRelayCommand<TModel> SubmitCommand { get; }

    event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    void SetSelectedItem(TModel e, bool isUpdate = true);
    Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true);
}
