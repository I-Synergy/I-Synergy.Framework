using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelDialog
/// Implements the <see cref="IViewModelSelectedItem{TEntity}" />
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
/// <seealso cref="IViewModelSelectedItem{TEntity}" />
public interface IViewModelDialog<TEntity> : IViewModelSelectedItem<TEntity>
{
    /// <summary>
    /// Occurs when [submitted].
    /// </summary>
    event EventHandler<SubmitEventArgs<TEntity>> Submitted;
}
