using System.Text.Json;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Class HttpContentExtensions.
/// </summary>
public static class HttpContentExtensions
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// read as as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content">The content.</param>
    /// <returns>T.</returns>
    public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
