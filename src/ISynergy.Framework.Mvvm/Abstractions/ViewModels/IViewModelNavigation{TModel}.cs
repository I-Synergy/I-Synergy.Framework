using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelNavigation
/// Implements the <see cref="IViewModelSelectedItem{TEntity}" />
/// </summary>
/// <typeparam name="TModel">The type of the t entity.</typeparam>
/// <seealso cref="IViewModelSelectedItem{TEntity}" />
public interface IViewModelNavigation<TModel> : IViewModelSelectedItem<TModel>
{
    event EventHandler<SubmitEventArgs<TModel>>? Submitted;

    void ApplyQueryAttributes(IDictionary<string, object> query);
    Task SubmitAsync(TModel e, bool validateUnderlayingProperties = true);
}
