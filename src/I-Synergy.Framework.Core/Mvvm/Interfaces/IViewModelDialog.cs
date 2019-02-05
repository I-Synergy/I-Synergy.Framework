using GalaSoft.MvvmLight.Command;
using System;

namespace ISynergy.ViewModels.Base
{
    public class ClosedWindowArgs : EventArgs
    {
        public IViewModel Result { get; set; }
    }

    public interface IViewModelDialog<TEntity> : IViewModel
    {
        TEntity SelectedItem { get; set; }
        bool IsNew { get; set; }
        RelayCommand<TEntity> Submit_Command { get; }
    }
}