using GalaSoft.MvvmLight.Command;
using ISynergy.Models.Base;
using ISynergy.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Base
{
    public class SubmittedItemArgs : EventArgs
    {
        public IViewModel Result { get; set; }
    }

    public interface IViewModel : IModelBase
    {
        IContext Context { get; }
        IBaseService BaseService { get; }

        bool CanClose { get; set; }
        bool IsCancelled { get; }
        string Title { get; }
        void OnDeactivate();
        void OnActivate(object parameter, bool isBack);
        RelayCommand Close_Command { get; }
       

        Task InitializeAsync();
        Task<bool> ValidateInputAsync();
    }
}