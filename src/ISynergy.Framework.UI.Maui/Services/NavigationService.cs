using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Extensions;

namespace ISynergy.Framework.UI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IContext _context;

        public event EventHandler BackStackChanged;

        /// <summary>
        /// Handles the <see cref="E:BackStackChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public virtual void OnBackStackChanged(EventArgs e) => BackStackChanged?.Invoke(this, e);

        /// <summary>
        /// The frame.
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// Navigation backstack.
        /// </summary>
        private Stack<object> _backStack = new Stack<object>();

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public object Frame 
        { 
            get => _frame; 
            set => _frame = (Frame)value; 
        }

        /// <summary>
        /// Gets a value indicating whether this instance can go back.
        /// </summary>
        /// <value><c>true</c> if this instance can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => _backStack.Count > 0 ? true : false;

        /// <summary>
        /// Gets a value indicating whether this instance can go forward.
        /// </summary>
        /// <value><c>true</c> if this instance can go forward; otherwise, <c>false</c>.</value>
        public bool CanGoForward => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="context"></param>
        public NavigationService(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Goes the back.
        /// </summary>
        public async Task GoBackAsync()
        {
            if (CanGoBack && _backStack.Pop() is IViewModel viewModel)
            {
                await NavigateAsync(viewModel, navigateBack: true);
            }
        }

        /// <summary>
        /// Goes the forward.
        /// </summary>
        public Task GoForwardAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Navigates to a specified viewmodel asynchronous.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="navigateBack"></param>
        /// <returns></returns>
        public Task NavigateAsync<TViewModel>(object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel =>
            NavigateAsync(default(TViewModel), parameter);

        /// <summary>
        /// Navigates to the viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="parameter"></param>
        /// <param name="navigateBack"></param>
        /// <returns></returns>
        public Task NavigateAsync<TViewModel>(TViewModel viewModel, object parameter = null, bool navigateBack = false) where TViewModel : class, IViewModel =>
            Shell.Current.Navigation.PushViewModelAsync<TViewModel>(parameter);

        /// <summary>
        /// Navigates to the modal viewmodel with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task NavigateModalAsync<TViewModel>(object parameter = null)
             where TViewModel : class, IViewModel
        {
            var page = await NavigationExtensions.CreatePage<TViewModel>(parameter);
            Application.Current.MainPage.Dispatcher.Dispatch(() => Application.Current.MainPage = page);
        }

        public Task CleanBackStackAsync()
        {
            _backStack.Clear();
            OnBackStackChanged(EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task OpenBladeAsync(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();

        public void RemoveBlade(IViewModelBladeView owner, IViewModel viewmodel) => throw new NotImplementedException();
    }
}
