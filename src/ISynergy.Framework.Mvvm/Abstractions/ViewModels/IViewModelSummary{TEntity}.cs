using System.Collections.ObjectModel;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels;

/// <summary>
/// Interface IViewModelSummary
/// Implements the <see cref="IViewModel" />
/// </summary>
/// <seealso cref="IViewModel" />
public interface IViewModelSummary<TEntity> : IViewModelSelectedItem<TEntity>
{
    ObservableCollection<TEntity> Items { get; set; }
}
