using ISynergy.Framework.Mvvm.Abstractions.Services;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// This dispatcher service is used to dispatch actions to the UI thread.
/// Because of the use of Application.Current.Dispatcher, this dispatcher cannot be used in a multi-window environment.
/// For multi-window environments, use the DispatcherService with the Window parameter.
/// DO NOT use EITHER MainThread nor Application.Current.Dispatcher in a multi-window app.
/// You MUST get hold of a UI object on the window being touched.
/// If you are writing code in a UI element, use this.Dispatcher. [this. is optional; shown for clarity.] OR pass in to your method a reference to any UI element of that window (someUIElement.Dispatcher).
/// </summary>
internal class DispatcherService : IDispatcherService
{
    private readonly IDispatcher _dispatcher;

    public object Dispatcher 
    {
        init
        {
            if (Application.Current is not null)
            {
                if (Application.Current.MainPage is not null)
                    _dispatcher = Application.Current.MainPage.Dispatcher;
                else
                    _dispatcher = Application.Current.Dispatcher;
            }
            else
                throw new NullReferenceException("Application.Current is null. Dispatcher cannot be used.");
        }
        get => _dispatcher;
    }

    public bool Invoke(Action action) =>
        _dispatcher.Dispatch(action);
}
