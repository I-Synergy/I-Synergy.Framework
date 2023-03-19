using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services
{
    internal class DispatcherService : IDispatcherService
    {
        public bool Invoke(Action action) =>
            Application.Current.MainPage.Dispatcher.Dispatch(action);
    }
}
