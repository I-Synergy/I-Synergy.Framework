using System.Diagnostics.CodeAnalysis;
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
    /// Reads the HTTP content as an instance of <typeparamref name="T"/> by deserializing the JSON body.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response content into.</typeparam>
    /// <param name="content">The content.</param>
    /// <returns>The deserialized instance of <typeparamref name="T"/>, or <c>null</c> if deserialization returns no result.</returns>
    [RequiresUnreferencedCode("JSON deserialization requires type metadata to be preserved. This method is not AOT-safe. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    [RequiresDynamicCode("JSON deserialization requires dynamic code generation. This method is not AOT-safe. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync().ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}
