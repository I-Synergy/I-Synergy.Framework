using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using ISynergy.Framework.Windows.Controls;
using ISynergy.Framework.Windows.Extensions;
using Windows.ApplicationModel.Core;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using System.Threading;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using System.Reflection;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.Windows.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="INavigationService" />
    /// </summary>
    /// <seealso cref="INavigationService" />
    public class NavigationService : INavigationService
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
        /// The frame
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public object Frame
        {
            get => _frame ??= (Frame)global::Windows.UI.Xaml.Window.Current.Content;
            set
            {
                _frame = (Frame)value;
            }
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
        /// <param name="infoOverride">The information override.</param>
        /// <returns>Task&lt;IView&gt;.</returns>
        /// <exception cref="ArgumentException">Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?</exception>
        public async Task<IView> NavigateAsync<TViewModel>(object parameter = null, object infoOverride = null)
            where TViewModel : class, IViewModel
        {
            await _semaphore.WaitAsync();

            try
            {
                IViewModel viewmodel = default;
                IView view = default;

                if(parameter is IViewModel instanceVM)
                {
                    viewmodel = instanceVM;
                }
                else
                {
                    viewmodel = ServiceLocator.Default.GetInstance<TViewModel>();
                }
                
                if (!_pages.ContainsKey(viewmodel.GetType().FullName))
                {
                    throw new ArgumentException($"Page not found: {viewmodel.GetType().FullName}. Did you forget to call NavigationService.Configure?", viewmodel.GetType().FullName);
                }

                var page = _pages[viewmodel.GetType().FullName];

                if (_frame is Frame frame)
                {
                    // Check if actual page is the same as destination page.
                    if (frame.Content != null && frame.Content.GetType().Equals(page))
                    {
                        return frame.Content as IView;
                    }

                    if (parameter is IViewModel)
                    {
                        viewmodel.IsInitialized = false;
                        await InitializeViewModelAsync(viewmodel);
                        view = frame.NavigateToView(page, viewmodel, (NavigationTransitionInfo)infoOverride);
                    }
                    else
                    {
                        if (page.GetInterfaces(true).Where(q => q == typeof(IView)).Any())
                        {
                            if (parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
                            {
                                Type genericPropertyType = null;

                                // Has class GenericTypeArguments?
                                if (viewmodel.GetType().GenericTypeArguments.Count() > 0)
                                {
                                    genericPropertyType = viewmodel.GetType().GetGenericArguments().First();
                                }

                                // Has BaseType GenericTypeArguments?
                                else if (viewmodel.GetType().BaseType is Type baseType && baseType.GenericTypeArguments.Count() > 0)
                                {
                                    genericPropertyType = baseType.GetGenericArguments().First();
                                }

                                if (genericPropertyType != null && parameter.GetType() == genericPropertyType)
                                {
                                    var genericInterfaceType = typeof(IViewModelSelectedItem<>).MakeGenericType(genericPropertyType);

                                    // Check if instanceVM implements genericInterfaceType.
                                    if (genericInterfaceType.IsAssignableFrom(viewmodel.GetType()) && viewmodel.GetType().GetMethod("SetSelectedItem") is MethodInfo method)
                                    {
                                        method.Invoke(viewmodel, new[] { parameter });
                                    }
                                }
                            }
                        }

                        await InitializeViewModelAsync(viewmodel);
                        view = frame.NavigateToView(page, viewmodel, (NavigationTransitionInfo)infoOverride);
                    }
                }

                return view;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// initialize view model as an asynchronous operation.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private async Task InitializeViewModelAsync(IViewModel viewModel)
        {
            if(!viewModel.IsInitialized)
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
            Argument.IsNotNull(nameof(owner), owner);

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

            owner.IsPaneEnabled = true;
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
            Argument.IsNotNull(nameof(owner), owner);

            if (owner.Blades != null)
            {
                if (owner.Blades.Remove(
                    owner.Blades
                        .Where(q =>
                            q.DataContext == viewmodel &&
                            ((IViewModelBlade)q.DataContext).Owner == viewmodel.Owner)
                        .FirstOrDefault())
                    )
                {
                    if (owner.Blades.Count < 1)
                    {
                        owner.IsPaneEnabled = false;
                    }
                    else if(owner.Blades.Last() is IView blade)
                    {
                        blade.IsEnabled = true;
                    }
                }
            }

            return Task.CompletedTask;
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
            await _semaphore.WaitAsync();

            try
            {
                var viewModelKey = viewModel.GetType().FullName;

                if (!_pages.ContainsKey(viewModelKey))
                {
                    throw new ArgumentException($"Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure?", nameof(viewModel));
                }

                if (!viewModel.IsInitialized)
                {
                    await viewModel.InitializeAsync();
                }

                if(TypeActivator.CreateInstance(_pages[viewModelKey]) is View view)
                {
                    var datacontextBinding = new Binding
                    {
                        Source = viewModel,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };

                    BindingOperations.SetBinding(view, FrameworkElement.DataContextProperty, datacontextBinding);
                    return view;
                }

                throw new ArgumentException($"Instance could not be created from {viewModelKey}");
            }
            finally
            {
                _semaphore.Release();
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

        /// <summary>
        /// clean back stack as an asynchronous operation.
        /// </summary>
        public async Task CleanBackStackAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ((Frame)Frame).BackStack.Clear();
            });
        }
    }
}
