using ISynergy.Framework.UI.Abstractions.Security;
using Microsoft.JSInterop;

namespace ISynergy.Framework.UI.Security;

/// <summary>
/// Creates <see cref="HttpClient"/> instances that carry the page's antiforgery token.
/// </summary>
public class AntiforgeryHttpClientFactory : IAntiforgeryHttpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _jSRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="AntiforgeryHttpClientFactory"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The factory used to create the named client.</param>
    /// <param name="jSRuntime">The JavaScript runtime used to read the token from the page.</param>
    public AntiforgeryHttpClientFactory(IHttpClientFactory httpClientFactory, IJSRuntime jSRuntime)
    {
        _httpClientFactory = httpClientFactory;
        _jSRuntime = jSRuntime;
    }

    /// <summary>
    /// Creates a client for the given name with the antiforgery token in the
    /// <c>X-XSRF-TOKEN</c> header.
    /// </summary>
    /// <param name="clientName">The name of the client registered with <see cref="IHttpClientFactory"/>.</param>
    /// <returns>The configured client.</returns>
    /// <exception cref="InvalidOperationException">
    /// The token could not be read through JavaScript interop — the circuit is not running yet, is
    /// disconnected, or the page script failed — or the page returned no token.
    /// </exception>
    public async Task<HttpClient> CreateClientAsync(string clientName = "authorizedClient")
    {
        string? token;

        try
        {
            token = await _jSRuntime.InvokeAsync<string>("getAntiForgeryToken");
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or InvalidOperationException)
        {
            // Deliberately fatal: a client without the token would only fail later with an opaque
            // server-side 400. InvalidOperationException is what interop throws while prerendering.
            throw new InvalidOperationException(
                "The antiforgery token could not be read through JavaScript interop. " +
                "The token is only available once the Blazor circuit has started.", ex);
        }

        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("The page did not provide an antiforgery token.");

        var client = _httpClientFactory.CreateClient(clientName);
        client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);

        return client;
    }
}
