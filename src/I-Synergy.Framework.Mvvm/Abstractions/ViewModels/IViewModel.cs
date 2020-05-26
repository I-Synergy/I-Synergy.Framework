using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface IViewModel : IObservableClass, ICleanup
    {
        event EventHandler Cancelled;
        event EventHandler Closed;

        IContext Context { get; }
        IBaseCommonServices BaseCommonServices { get; }
        ILogger Logger { get; }
        RelayCommand Close_Command { get; }
        bool CanClose { get; set; }
        bool IsCancelled { get; }
        string Title { get; }
        Task OnDeactivateAsync();
        Task OnActivateAsync(object parameter, bool isBack);
        Task InitializeAsync();
        bool IsInitialized { get; set; }
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}
