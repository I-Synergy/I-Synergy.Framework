using ISynergy.Mvvm;
using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public interface IUIVisualizerService
    {
        Task<bool?> ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null) 
            where TWindow : IWindow 
            where TViewModel : IViewModelDialog<TEntity>
            where TEntity : class, new();
            

        Task<bool?> ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
            where TEntity : class, new();

        Task<bool?> ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
            where TEntity : class, new();
    }
}