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
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private Frame _frame;

        public object Frame
        {
            get => _frame ?? (_frame = (Frame)global::Windows.UI.Xaml.Window.Current.Content);
            set
            {
                _frame = (Frame)value;
            }
        }

        public bool CanGoBack => _frame.CanGoBack;
        public bool CanGoForward => _frame.CanGoForward;

        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }
        
        public void GoForward()
        {
            if (_frame.CanGoForward)
                _frame.GoForward();
        }

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

        private async Task InitializeViewModelAsync(IViewModel viewModel)
        {
            if(!viewModel.IsInitialized)
            {
                await viewModel.InitializeAsync();
            }
        }

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

        private async void Viewmodel_Closed(object sender, EventArgs e)
        {
            if (sender is IViewModelBlade viewModel)
                await RemoveBladeAsync(viewModel.Owner, viewModel);
        }

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
