using GalaSoft.MvvmLight.Command;
using ISynergy.Models.Base;
using ISynergy.Services;
using System;

namespace ISynergy.ViewModels.Base
{
    public class SubmittedItemArgs : EventArgs
    {
        public IViewModel Result { get; set; }
    }

    public interface IViewModel : IBaseModel
    {
        IContext Context { get; }
        ISynergyService SynergyService { get; }

        bool CanClose { get; set; }
        bool IsCancelled { get; }
        string Title { get; }
        void OnDeactivate();
        void OnActivate(object parameter, bool isBack);
        RelayCommand Close_Command { get; }
    }
}