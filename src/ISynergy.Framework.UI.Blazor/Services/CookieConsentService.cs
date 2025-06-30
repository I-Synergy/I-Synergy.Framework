using ISynergy.Framework.UI.Cookies;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using Microsoft.JSInterop;

namespace ISynergy.Framework.UI.Services;

public class CookieConsentService(IOptions<AnalyticOptions> analyticOptions, IJSRuntime jsRuntime) : JSModule(jsRuntime, "./_content/ISynergy.Framework.UI.Blazor/Components/Cookies/CookieConsent.razor.js")
{
    private CookieState? _cookieState;

    public async Task<bool> IsAnalyticsConsentedAsync() =>
        (await GetCookieStateAsync())?.AcceptAnalytics == true;

    public async Task<bool> IsSocialMediaConsentedAsync() =>
        (await GetCookieStateAsync())?.AcceptSocialMedia == true;

    public async Task<bool> IsAdvertisingConsentedAsync() =>
        (await GetCookieStateAsync())?.AcceptAdvertising == true;

    public async Task<bool> IsConsentGivenAsync() =>
        await GetCookieStateAsync() != null;

    public async Task<CookieState?> GetCookieStateAsync()
    {
        _cookieState ??= await InvokeAsync<CookieState>("getCookiePolicy");
        return _cookieState;
    }

    public async Task SetCookieStateAsync(CookieState state)
    {
        await InvokeVoidAsync("setCookiePolicy", state);
    }

    public async Task InitAnalyticsAsync()
    {
        var analytics = analyticOptions.Value;

        if (analytics is not null)
            await InvokeVoidAsync("initAnalytics", analytics.GoogleAnalyticsMeasurementId ?? string.Empty, analytics.MicrosoftClarityProjectId ?? string.Empty);
        else
            await InvokeVoidAsync("initAnalytics", string.Empty, string.Empty);
    }

}