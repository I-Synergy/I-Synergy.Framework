using GalaSoft.MvvmLight.Command;
using ISynergy.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISynergy.Mvvm
{
    public class SubmittedItemArgs : EventArgs
    {
        public IViewModel Result { get; set; }
    }

    public interface IViewModel : IModelBase
    {
        IContext Context { get; }
        IBaseService BaseService { get; }

        RelayCommand Close_Command { get; }

        bool CanClose { get; set; }
        bool IsCancelled { get; }
        string Title { get; }
        Task OnDeactivateAsync();
        Task OnActivateAsync(object parameter, bool isBack);
        Task InitializeAsync();
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
        void Cleanup();
    }
}