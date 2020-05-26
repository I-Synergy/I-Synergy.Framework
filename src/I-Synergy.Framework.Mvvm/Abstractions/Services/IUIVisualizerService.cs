using System;
using System.Threading.Tasks;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    public interface IUIVisualizerService
    {
        Task<bool> ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>;

        Task<bool> ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel);
        Task<bool> ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel);
    }
}
