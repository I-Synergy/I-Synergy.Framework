using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Models.Results;
using System.Text.Json;

namespace ISynergy.Framework.AspNetCore.Extensions;

public static class ResultExtensions
{
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = null
    };

    public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, _options);
        return responseObject;
    }

    public static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, _options);
        return responseObject;
    }

    public static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, _options);
        return responseObject;
    }
}
