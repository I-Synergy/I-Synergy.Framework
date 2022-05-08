using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="INavigationService" />
    /// </summary>
    /// <seealso cref="INavigationService" />
    public partial class NavigationService : INavigationService
    {
        /// <summary>
        /// The pages
        /// </summary>
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        /// <summary>
        /// The semaphore
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// initialize view model as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private async Task InitializeViewModelAsync(IViewModel viewModel)
        {
            if (!viewModel.IsInitialized)
            {
                await viewModel.InitializeAsync();
            }
        }

        /// <summary>
        /// open blade as an asynchronous operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel)
        {
            Argument.IsNotNull(owner);

            viewmodel.Owner = owner;
            viewmodel.Closed += Viewmodel_Closed;

            var view = await GetNavigationBladeAsync(viewmodel);

            if (!owner.Blades.Any(a => a.GetType().FullName.Equals(view.GetType().FullName)))
            {
                foreach (var blade in owner.Blades)
                {
                    blade.IsEnabled = false;
                }

                owner.Blades.Add(view);
            }

            owner.IsPaneVisible = true;
        }

        /// <summary>
        /// Handles the Closed event of the Viewmodel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void Viewmodel_Closed(object sender, EventArgs e)
        {
            if (sender is IViewModelBlade viewModel)
                await RemoveBladeAsync(viewModel.Owner, viewModel);
        }

        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public Task RemoveBladeAsync(IViewModelBladeView owner, IViewModelBlade viewmodel)
        {
            Argument.IsNotNull(owner);

            if (owner.Blades is not null)
            {
                if (owner.Blades.Remove(
                    owner.Blades
                        .FirstOrDefault(q =>
                            q.ViewModel == viewmodel &&
                            ((IViewModelBlade)q.ViewModel).Owner == viewmodel.Owner))
                    )
                {
                    if (owner.Blades.Count < 1)
                    {
                        owner.IsPaneVisible = false;
                    }
                    else if (owner.Blades.Last() is IView blade)
                    {
                        blade.IsEnabled = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Configures the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pageType">Type of the page.</param>
        /// <exception cref="ArgumentException">The key {key} is already configured in NavigationService</exception>
        /// <exception cref="ArgumentException">This type is already configured with key {_pages.First(p => p.Value == pageType).Key}</exception>
        public void Configure(string key, Type pageType)
        {
            _semaphore.Wait();

            try
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
                }

                _pages.Add(key, pageType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Gets the name of registered page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The page '{page.Name}' is unknown by the NavigationService</exception>
        public string GetNameOfRegisteredPage(Type page)
        {
            _semaphore.Wait();

            try
            {
                if (_pages.ContainsValue(page))
                {
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                }
                else
                {
                    throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
