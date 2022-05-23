namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    /// <summary>
    /// Interface IViewModelNavigation
    /// Implements the <see cref="IViewModelSelectedItem{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="IViewModelSelectedItem{TEntity}" />
    public interface IViewModelNavigation<TEntity> : IViewModelSelectedItem<TEntity>
    {
    }
}
