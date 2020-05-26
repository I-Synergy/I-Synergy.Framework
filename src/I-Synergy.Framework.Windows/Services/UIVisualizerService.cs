using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISynergy.Framework.Windows.Controls;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Core.Locators;

namespace ISynergy.Framework.Windows.Services
{
    public class UIVisualizerService : IUIVisualizerService
    {
        public List<Window> RegisteredWindows { get; internal set; }

        private static bool IsShown = false;

        public UIVisualizerService()
        {
            RegisteredWindows = new List<Window>();
        }

        public Task<bool> ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            if (viewmodel is null) viewmodel = (IViewModelDialog<TEntity>)ServiceLocator.Default.GetInstance(typeof(TViewModel));

            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(typeof(TWindow)), viewmodel);
        }

        public Task<bool> ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
        {
            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(window.GetType()), viewmodel);
        }

        public Task<bool> ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
        {
            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(type), viewmodel);
        }

        private async Task<bool> CreateDialogAsync<TEntity>(Window dialog, IViewModelDialog<TEntity> viewmodel)
        {
            bool result = false;

            if (!RegisteredWindows.Any(q => q.Name.Equals(dialog.Name)))
            {
                dialog.DataContext = viewmodel;
                dialog.PrimaryButtonCommand = viewmodel.Submit_Command;
                dialog.SecondaryButtonCommand = viewmodel.Close_Command;
                dialog.CloseButtonCommand = viewmodel.Close_Command;

                viewmodel.Closed += (sender, e) => dialog.Close();
                viewmodel.Submitted += (sender, e) => dialog.Close();

                if (!viewmodel.IsInitialized)
                    await viewmodel.InitializeAsync();

                RegisteredWindows.Add(dialog);

                if (IsShown)
                    return result;

                IsShown = true;

                for (var i = 0; i < RegisteredWindows.Count(q => q.Equals(dialog)); i++)
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
