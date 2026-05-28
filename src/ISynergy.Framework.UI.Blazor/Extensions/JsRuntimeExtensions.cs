using Microsoft.JSInterop;

namespace ISynergy.Framework.UI.Extensions;

public static class JsRuntimeExtensions
{
    public static async Task FocusElementById(this IJSRuntime jsRuntime, string id)
    {
        var element = await jsRuntime.InvokeAsync<IJSObjectReference>("document.getElementById", id);
        await element.InvokeVoidAsync("focus");
    }
}
