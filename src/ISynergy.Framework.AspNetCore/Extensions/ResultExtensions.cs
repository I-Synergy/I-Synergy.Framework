using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Serializers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ISynergy.Framework.AspNetCore.Extensions;

/// <summary>
/// Extension methods for converting <see cref="HttpResponseMessage"/> content to Result types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Deserializes the response content as a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>A <see cref="IResult{T}"/> deserialized from the response body, or <c>null</c> if deserialization fails.</returns>
    [RequiresUnreferencedCode("JSON deserialization of open-generic Result types requires metadata preservation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    [RequiresDynamicCode("JSON deserialization of open-generic Result types requires dynamic code generation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    public static async Task<IResult<T>?> ToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, DefaultJsonSerializers.Web);
        return responseObject;
    }

    /// <summary>
    /// Deserializes the response content as a <see cref="Result"/>.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>A <see cref="IResult"/> deserialized from the response body, or <c>null</c> if deserialization fails.</returns>
    [RequiresUnreferencedCode("JSON deserialization of Result requires metadata preservation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    [RequiresDynamicCode("JSON deserialization of Result requires dynamic code generation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    public static async Task<IResult?> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, DefaultJsonSerializers.Web);
        return responseObject;
    }

    /// <summary>
    /// Deserializes the response content as a <see cref="PaginatedResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The result item type.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>A <see cref="PaginatedResult{T}"/> deserialized from the response body, or <c>null</c> if deserialization fails.</returns>
    [RequiresUnreferencedCode("JSON deserialization of open-generic PaginatedResult types requires metadata preservation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    [RequiresDynamicCode("JSON deserialization of open-generic PaginatedResult types requires dynamic code generation. Suppress this warning or use a JsonTypeInfo<T>-aware overload in AOT-published code.")]
    public static async Task<PaginatedResult<T>?> ToPaginatedResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, DefaultJsonSerializers.Web);
        return responseObject;
    }
}
