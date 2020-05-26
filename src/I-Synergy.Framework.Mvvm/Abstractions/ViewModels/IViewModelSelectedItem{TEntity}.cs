using ISynergy.Framework.Mvvm.Commands;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IViewModelSelectedItem<TEntity> : IViewModel 
    {
        bool IsNew { get; set; }
        void SetSelectedItem(TEntity entity);
        TEntity SelectedItem { get; }
        RelayCommand<TEntity> Submit_Command { get; }
    }
}
