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
    /// <summary>
    /// Class UIVisualizerService.
    /// Implements the <see cref="IUIVisualizerService" />
    /// </summary>
    /// <seealso cref="IUIVisualizerService" />
    public class UIVisualizerService : IUIVisualizerService
    {
        /// <summary>
        /// Gets the registered windows.
        /// </summary>
        /// <value>The registered windows.</value>
        public List<Window> RegisteredWindows { get; internal set; }

        /// <summary>
        /// The is shown
        /// </summary>
        private static bool IsShown = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIVisualizerService"/> class.
        /// </summary>
        public UIVisualizerService()
        {
            RegisteredWindows = new List<Window>();
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TWindow">The type of the t window.</typeparam>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ShowDialogAsync<TWindow, TViewModel, TEntity>(IViewModelDialog<TEntity> viewmodel = null)
            where TWindow : IWindow
            where TViewModel : IViewModelDialog<TEntity>
        {
            if (viewmodel is null) viewmodel = (IViewModelDialog<TEntity>)ServiceLocator.Default.GetInstance(typeof(TViewModel));

            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(typeof(TWindow)), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="window">The window.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ShowDialogAsync<TEntity>(IWindow window, IViewModelDialog<TEntity> viewmodel)
        {
            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(window.GetType()), viewmodel);
        }

        /// <summary>
        /// Shows the dialog asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ShowDialogAsync<TEntity>(Type type, IViewModelDialog<TEntity> viewmodel)
        {
            return CreateDialogAsync((Window)ServiceLocator.Default.GetInstance(type), viewmodel);
        }

        /// <summary>
        /// create dialog as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <param name="dialog">The dialog.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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
