using Microsoft.JSInterop;

namespace ISynergy.Framework.AspNetCore.Blazor.Extensions;

public static class JsRuntimeExtensions
{
    /// <summary>
    /// Moves the keyboard focus to the element with the given id.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime of the current circuit.</param>
    /// <param name="id">The DOM id of the element to focus.</param>
    /// <remarks>
    /// Best effort: focusing is a convenience and must never take the app down, so the interop
    /// failures (a script error, a disconnected circuit, or prerendering where JavaScript is not
    /// available yet) are swallowed instead of propagated to the caller.
    /// </remarks>
    public static async Task FocusElementById(this IJSRuntime jsRuntime, string id)
    {
        try
        {
            var element = await jsRuntime.InvokeAsync<IJSObjectReference>("document.getElementById", id);

            if (element is null)
                return;

            try
            {
                await element.InvokeVoidAsync("focus");
            }
            finally
            {
                // The browser keeps the DOM node; the .NET handle is not needed afterwards.
                await element.DisposeAsync();
            }
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or InvalidOperationException)
        {
            // Focus is best effort. InvalidOperationException is what interop throws while
            // prerendering, before the circuit has JavaScript available.
        }
    }
}
