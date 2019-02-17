using GalaSoft.MvvmLight.Command;

namespace ISynergy.Mvvm
{
    public interface IViewModelNavigation<TEntity> : IViewModel
    {
        TEntity SelectedItem { get; set; }
        bool IsNew { get; set; }
        RelayCommand<TEntity> Submit_Command { get; }
    }
}
