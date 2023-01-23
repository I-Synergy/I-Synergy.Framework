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
        /// <param name="viewModel"></param>
        protected View(IViewModel viewModel)
            : this()
        {
            ViewModel = viewModel;
            Loaded += View_Loaded;
        }

        private async void View_Loaded(object sender, EventArgs e)
        {
            Loaded -= View_Loaded;

            if (ViewModel is not null)
                await ViewModel.InitializeAsync();
        }
    }
}
