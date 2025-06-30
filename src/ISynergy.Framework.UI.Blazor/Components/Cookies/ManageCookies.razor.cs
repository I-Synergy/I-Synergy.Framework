using ISynergy.Framework.UI.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace ISynergy.Framework.UI.Components.Cookies;

public partial class ManageCookies
{
    private bool _buttonsDisabled => Content.AcceptAnalytics is null && Content.AcceptSocialMedia is null && Content.AcceptAdvertising is null;

    [Parameter]
    public CookieState Content { get; set; } = default!;

    [CascadingParameter]
    public FluentDialog Dialog { get; set; } = default!;


    private async Task HandleSaveAsync()
    {
        await Dialog.CloseAsync(Content);
    }

    private async Task HandleCancelAsync()
    {
        Content.AcceptAnalytics = null;
        Content.AcceptSocialMedia = null;
        Content.AcceptAdvertising = null;
        await Dialog.CancelAsync();
    }
}
