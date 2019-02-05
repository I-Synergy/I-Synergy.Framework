using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace ISynergy.Mvvm
{
    public interface IViewModelBladeView<TEntity> : IViewModelNavigation<TEntity>
    {
        RelayCommand<TEntity> Edit_Command { get; set; }
        ObservableCollection<TEntity> Items { get; set; }
    }
}