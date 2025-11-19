using ISynergy.Framework.Mvvm.Commands;
using System.ComponentModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelDialogWizard
/// Implements the <see cref="IViewModelDialog{TEntity}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="IViewModelDialog{TEntity}" />
public interface IViewModelDialogWizard<TModel> : IViewModelDialog<TModel> 
{
    bool Back_IsEnabled { get; set; }
    RelayCommand BackCommand { get; }
    bool Next_IsEnabled { get; set; }
    RelayCommand NextCommand { get; }
    int Page { get; set; }
    int Pages { get; set; }
    bool Submit_IsEnabled { get; set; }
}
