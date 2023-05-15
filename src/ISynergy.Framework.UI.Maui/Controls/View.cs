using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;

namespace ISynergy.Framework.UI.Controls
{
    public abstract class View : ContentPage, IView
    {
        private IViewModel _viewModel;

        /// <summary>
        /// Gets or sets the viewmodel and data context for a view.
        /// </summary>
        /// <value>The data context.</value>
        public IViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                BindingContext = _viewModel;
                SetBinding(View.TitleProperty, new Binding(nameof(ViewModel.Title)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the view class.
        /// </summary>
        protected View()
        {
        }

        /// <summary>
        /// Initializes a new instance of the view class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewModelType"></param>
        protected View(IContext context, Type viewModelType)
            : this()
        {
            ViewModel = context.ScopedServices.ServiceProvider.GetRequiredService(viewModelType) as IViewModel;
            Loaded += async (sender, e) =>
            {
                if (ViewModel is not null)
                    await ViewModel.InitializeAsync();
            };
        }
    }
}
