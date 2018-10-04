using DryIoc;
using ISynergy.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public abstract class UIVisualizerServiceBase : IUIVisualizerService
    {
        public IContainer Container { get; }

        public List<ISynergy.Controls.Window> RegisteredWindows { get; internal set; }

        private static bool IsShown = false;

        public UIVisualizerServiceBase(IContainer container)
        {
            Container = container;
            RegisteredWindows = new List<ISynergy.Controls.Window>();
        }

        public Task<bool?> ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
            where TEntity : class, new()
        {
            if (viewmodel is null) viewmodel = (IViewModelDialog<TEntity>)Container.Resolve(typeof(TViewModel));

            return ShowDialogAsync((ISynergy.Controls.Window)Container.Resolve(typeof(TWindow)), viewmodel);
        }

        public Task<bool?> ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
            where TEntity : class, new()
        {
            return ShowDialogAsync((ISynergy.Controls.Window)Container.Resolve(window.GetType()), viewmodel);
        }

        public Task<bool?> ShowDialogAsync<TEntity>(Type window, IViewModelDialog<TEntity> viewmodel)
            where TEntity : class, new()
        {
            return ShowDialogAsync((ISynergy.Controls.Window)Container.Resolve(window), viewmodel);
        }

        public virtual async Task<bool?> ShowDialogAsync<TEntity>(ISynergy.Controls.Window dialog, IViewModelDialog<TEntity> viewmodel)
            where TEntity : class, new()
        {
            bool? result = false;

            if (!RegisteredWindows.Any(q => q.Name.Equals(dialog.Name)))
            {
                dialog.DataContext = viewmodel;
                dialog.PrimaryButtonCommand = viewmodel.Submit_Command;
                dialog.SecondaryButtonCommand = viewmodel.Close_Command;
                dialog.CloseButtonCommand = viewmodel.Close_Command;

                RegisteredWindows.Add(dialog);

                if (IsShown)
                    return result;

                IsShown = true;

                for (int i = 0; i < RegisteredWindows.Count(q => q.Equals(dialog)); i++)
                {
                    result = await RegisteredWindows[i].ShowAsync<TEntity>();
                    RegisteredWindows.Remove(RegisteredWindows[i]);
                    i--;
                }

                IsShown = false;
            }
            
            return result;
        }
    }
}