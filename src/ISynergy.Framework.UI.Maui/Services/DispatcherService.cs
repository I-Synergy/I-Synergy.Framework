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
public class DispatcherService : IDispatcherService
{
    public bool Invoke(Action action)
    {
        if (Application.Current.MainPage is not null && Application.Current.MainPage.Dispatcher is IDispatcher dispatcher)
            return dispatcher.Dispatch(action);
        else
            return Application.Current.Dispatcher.Dispatch(action);
    }
}
