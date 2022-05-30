using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class NavigationService.
    /// Implements the <see cref="INavigationService" />
    /// </summary>
    /// <seealso cref="INavigationService" />
    public partial class NavigationService
    {
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
            get => _frame ??= (Frame)Application.Current.MainWindow.Content;
            set => _frame = (Frame)value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => ((Frame)Frame).CanGoBack;
        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public bool CanGoForward => ((Frame)Frame).CanGoForward;

        /// <summary>
        /// Goes the back.
        /// </summary>
        public void GoBack()
        {
            if (((Frame)Frame).CanGoBack)
                ((Frame)Frame).GoBack();
        }

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public void GoForward()
        {
            if (((Frame)Frame).CanGoForward)
                ((Frame)Frame).GoForward();
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

                if (parameter is IViewModel instanceVM)
                {
                    viewmodel = instanceVM;
                }
                else
                {
                    viewmodel = ServiceLocator.Default.GetInstance<TViewModel>();
                }

                var viewModelKey = viewmodel.GetViewModelFullName();

                if (!_pages.ContainsKey(viewModelKey))
                {
                    throw new ArgumentException($"Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure?", viewModelKey);
                }

                var page = _pages[viewModelKey];

                // Check if actual page is the same as destination page.
                if (((Frame)Frame).Content is not null && ((Frame)Frame).Content.GetType().Equals(page))
                {
                    return ((Frame)Frame).Content as IView;
                }

                if (parameter is IViewModel)
                {
                    viewmodel.IsInitialized = false;
                    await InitializeViewModelAsync(viewmodel);
                    return ((Frame)Frame).NavigateToView(page, viewmodel);
                }
                else
                {
                    if (page.GetInterfaces(true).Any(q => q == typeof(IView)))
                    {
                        if (parameter is not null && !string.IsNullOrEmpty(parameter.ToString()))
                        {
                            Type genericPropertyType = null;

                            // Has class GenericTypeArguments?
                            if (viewmodel.GetType().GenericTypeArguments.Any())
                            {
                                genericPropertyType = viewmodel.GetType().GetGenericArguments().First();
                            }

                            // Has BaseType GenericTypeArguments?
                            else if (viewmodel.GetType().BaseType is Type baseType && baseType.GenericTypeArguments.Any())
                            {
                                genericPropertyType = baseType.GetGenericArguments().First();
                            }

                            if (genericPropertyType is not null && parameter.GetType() == genericPropertyType)
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
                    return ((Frame)Frame).NavigateToView(page, viewmodel);
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
            await _semaphore.WaitAsync();

            try
            {
                var viewModelKey = viewModel.GetViewModelFullName();

                if (!_pages.ContainsKey(viewModelKey))
                {
                    throw new ArgumentException($"Page not found: {viewModelKey}. Did you forget to call NavigationService.Configure?", nameof(viewModel));
                }

                if (!viewModel.IsInitialized)
                {
                    await viewModel.InitializeAsync();
                }

                if (TypeActivator.CreateInstance(_pages[viewModelKey]) is ISynergy.Framework.UI.Controls.View view)
                {
                    view.ViewModel = viewModel;
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
        /// clean back stack as an asynchronous operation.
        /// </summary>
        public Task CleanBackStackAsync()
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var item in (((Frame)Frame)).BackStack)
                {
                    (((Frame)Frame)).RemoveBackEntry();
                }
            }).Task;
        }
    }
}
