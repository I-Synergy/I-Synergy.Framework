using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="IBaseNavigationService" />
    /// </summary>
    /// <seealso cref="IBaseNavigationService" />
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// The frame
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// The pages
        /// </summary>
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        /// <summary>
        /// The semaphore
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public object Frame
        {
            get => _frame ??= (Frame)Application.Current.MainWindow.Content;
            set => _frame = (Frame)value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => _frame.CanGoBack;
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public bool CanGoForward => _frame.CanGoForward;

        /// <summary>
        /// Goes the back.
        /// </summary>
        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public void GoForward()
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }

        /// <summary>
        /// navigate as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
        public void Navigate<TViewModel>(object parameter = null)
            where TViewModel : class, IViewModel
        {
            _semaphore.Wait();

            try
            {
                IViewModel viewmodel = default;

                if (parameter is IViewModel instance)
                    viewmodel = instance;
                else
                    viewmodel = ServiceLocator.Default.GetInstance<TViewModel>();

                var viewModelKey = viewmodel.GetViewModelFullName();

                if (!_pages.ContainsKey(viewModelKey))
                    throw new Exception($"Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure?");

                var page = _pages[viewModelKey];

                // Check if actual page is the same as destination page.
                if (_frame.Content is not null && _frame.Content.GetType().Equals(page))
                    return;

                if (parameter is IViewModel)
                {
                    _frame.NavigateToView(page, viewmodel);
                }
                else
                {
                    if (page.GetInterfaces(true).Any(q => q == typeof(IView)) && parameter is not null && !string.IsNullOrEmpty(parameter.ToString()))
                    {
                        Type genericPropertyType = null;

                        // Has class GenericTypeArguments?
                        if (viewmodel.GetType().GenericTypeArguments.Any())
                            genericPropertyType = viewmodel.GetType().GetGenericArguments().First();
                        // Has BaseType GenericTypeArguments?
                        else if (viewmodel.GetType().BaseType is Type baseType && baseType.GenericTypeArguments.Any())
                            genericPropertyType = baseType.GetGenericArguments().First();

                        if (genericPropertyType is not null && parameter.GetType() == genericPropertyType)
                        {
                            var genericInterfaceType = typeof(IViewModelSelectedItem<>).MakeGenericType(genericPropertyType);

                            // Check if instance implements genericInterfaceType.
                            if (genericInterfaceType.IsInstanceOfType(viewmodel.GetType()) && viewmodel.GetType().GetMethod("SetSelectedItem") is MethodInfo method)
                                method.Invoke(viewmodel, new[] { parameter });
                        }
                    }

                    _frame.NavigateToView(page, viewmodel);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// get navigation blade as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>IView.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure? - viewModel</exception>
        /// <exception cref="ArgumentException">Instance could not be created from {viewModelKey}</exception>
        private async Task<IView> GetNavigationBladeAsync(IViewModel viewModel)
        {
            _semaphore.Wait();

            try
            {
                var viewModelKey = viewModel.GetViewModelFullName();

                if (!_pages.ContainsKey(viewModelKey))
                    throw new Exception($"Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure?");

                //if (!viewModel.IsInitialized)
                //    await viewModel.InitializeAsync();

                if (TypeActivator.CreateInstance(_pages[viewModelKey]) is ISynergy.Framework.UI.Controls.View view)
                {
                    view.ViewModel = viewModel;
                    await viewModel.InitializeAsync();
                    return view;
                }

                throw new Exception($"Instance could not be created from {viewModelKey}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// open blade as an asynchronous operation.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public async Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel)
        {
            Argument.IsNotNull(owner);

            if (viewmodel is IViewModelBlade bladeVm)
            {
                bladeVm.Owner = owner;
                bladeVm.Closed += Viewmodel_Closed;

                var view = await GetNavigationBladeAsync(bladeVm);

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
        }

        /// <summary>
        /// Handles the Closed event of the Viewmodel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Viewmodel_Closed(object sender, EventArgs e)
        {
            if (sender is IViewModelBlade viewModel)
                RemoveBlade(viewModel.Owner, viewModel);
        }

        /// <summary>
        /// Removes the blade asynchronous.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="viewmodel">The viewmodel.</param>
        /// <returns>Task.</returns>
        public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel)
        {
            Argument.IsNotNull(owner);

            if (viewmodel is IViewModelBlade bladeVm)
            {
                if (owner.Blades is not null)
                {
                    if (owner.Blades.Remove(
                        owner.Blades
                            .FirstOrDefault(q =>
                                q.ViewModel == bladeVm &&
                                ((IViewModelBlade)q.ViewModel).Owner == bladeVm.Owner))
                        )
                    {
                        if (owner.Blades.Count < 1)
                            owner.IsPaneVisible = false;
                        else if (owner.Blades.Last() is IView blade)
                            blade.IsEnabled = true;
                    }
                }
            }
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
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");

                if (_pages.Any(p => p.Value == pageType))
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");

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
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                else
                    throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        Task IBaseNavigationService.NavigateAsync<TViewModel>()
        {
            this.Navigate<TViewModel>();
            return Task.CompletedTask;
        }

        Task IBaseNavigationService.NavigateAsync<TViewModel>(object parameter)
        {
            this.Navigate<TViewModel>(parameter);
            return Task.CompletedTask;
        }

        public Task NavigateAsync(Type viewModel)
        {
            throw new NotImplementedException();
        }

        public Task CleanBackStackAsync()
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in ((Frame)Frame).BackStack)
                {
                    ((Frame)Frame).RemoveBackEntry();
                }
            }).Task;
        }

        public Task ReplaceMainWindowAsync<T>() where T : IView
        {
            if (ServiceLocator.Default.GetInstance<T>() is Page page)
                Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");

            return Task.CompletedTask;
        }

        public Task ReplaceMainFrameAsync<T>() where T : IView
        {
            if (ServiceLocator.Default.GetInstance<T>() is Page page)
                Application.Current.Dispatcher.Invoke(() => _frame.Content = page);
            else
                throw new InvalidCastException($"Implementation of '{nameof(T)}' is not of type of Page.");

            return Task.CompletedTask;
        }
    }
}
