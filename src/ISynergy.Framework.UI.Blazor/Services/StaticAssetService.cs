using ISynergy.Framework.UI.Abstractions.Services;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace ISynergy.Framework.UI.Services;

public class StaticAssetService : IStaticAssetService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheStorageService _cacheStorageAccessor;

    public StaticAssetService(HttpClient httpClient, NavigationManager navigationManager, ICacheStorageService cacheStorageAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress ??= new Uri(navigationManager.BaseUri);
        _cacheStorageAccessor = cacheStorageAccessor;
    }

    public async Task<string?> GetAsync(string assetUrl, bool useCache = true)
    {
        string? result = null;

        if (!IsAllowedAssetUrl(assetUrl, _httpClient.BaseAddress))
            throw new ArgumentException(
                $"Asset URL '{assetUrl}' is not allowed. Only relative paths and URLs matching the application base address are permitted.",
                nameof(assetUrl));

        var message = CreateMessage(assetUrl);

        if (useCache)
        {
            // Get the result from the cache
            result = await _cacheStorageAccessor.GetAsync(message);
        }

        if (string.IsNullOrEmpty(result))
        {
            //It not in the cache (or cache not used), download the asset
            var response = await _httpClient.SendAsync(message);

            // If successful, store the response in the cache and get the result
            if (response.IsSuccessStatusCode)
            {
                if (useCache)
                {
                    // Store the response in the cache and get the result
                    result = await _cacheStorageAccessor.PutAndGetAsync(message, response);
                }
                else
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            else
            {
                result = string.Empty;
            }
        }

        return result;
    }

    private static HttpRequestMessage CreateMessage(string url) => new(HttpMethod.Get, url);

    /// <summary>
    /// Determines whether the asset URL is safe to request, guarding against Server-Side Request Forgery (SSRF).
    /// </summary>
    /// <param name="url">The asset URL to validate.</param>
    /// <param name="baseAddress">The application base address; absolute URLs must match this host.</param>
    /// <returns><c>true</c> if the URL is a safe relative path or an absolute URL pointing to the same host as the application; otherwise <c>false</c>.</returns>
    internal static bool IsAllowedAssetUrl(string url, Uri? baseAddress)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Reject scheme-relative URLs (e.g. "//evil.example/path" or "\\server\share") which
        // are not parsed as absolute URIs but are resolved by HttpClient against their own host,
        // bypassing the application base-address restriction.
        if (url.StartsWith("//", StringComparison.Ordinal) || url.StartsWith("\\", StringComparison.Ordinal))
            return false;

        // Relative URLs are always safe — the HttpClient BaseAddress constrains the target.
        if (!Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            return true;

        // Only allow http/https schemes.
        if (absoluteUri.Scheme != Uri.UriSchemeHttp && absoluteUri.Scheme != Uri.UriSchemeHttps)
            return false;

        // Block loopback and link-local addresses to prevent SSRF against internal services.
        if (absoluteUri.HostNameType == UriHostNameType.IPv4 || absoluteUri.HostNameType == UriHostNameType.IPv6)
        {
            if (IPAddress.TryParse(absoluteUri.Host, out var ip) && (IPAddress.IsLoopback(ip) || IsPrivateAddress(ip)))
                return false;
        }
        else if (absoluteUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // If an absolute URL is supplied, it must match the application's own host/port.
        if (baseAddress is not null)
            return string.Equals(absoluteUri.Host, baseAddress.Host, StringComparison.OrdinalIgnoreCase)
                   && absoluteUri.Port == baseAddress.Port;

        return true;
    }

    private static bool IsPrivateAddress(IPAddress address)
    {
        // IPv4 private ranges: 10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16, 169.254.0.0/16
        var bytes = address.GetAddressBytes();

        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return bytes[0] == 10
                || (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                || (bytes[0] == 192 && bytes[1] == 168)
                || (bytes[0] == 169 && bytes[1] == 254);
        }

        // IPv6 link-local: fe80::/10
        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            return bytes[0] == 0xFE && (bytes[1] & 0xC0) == 0x80;

        return false;
    }
}
