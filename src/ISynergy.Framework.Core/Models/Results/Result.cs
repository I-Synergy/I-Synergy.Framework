using ISynergy.Framework.Core.Abstractions;
using System.Net;

namespace ISynergy.Framework.Core.Models.Results;

public class Result : IResult
{
    public Result() { }

    public List<string> Messages { get; set; } = [];

    public bool Succeeded { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code associated with this result (for API responses).
    /// </summary>
    public HttpStatusCode? StatusCode { get; set; }

    // Existing methods
    public static IResult Fail() =>
        new Result { Succeeded = false };

    public static IResult Fail(string message) =>
        new Result { Succeeded = false, Messages = [message] };

    public static IResult Fail(List<string> messages) =>
        new Result { Succeeded = false, Messages = messages };

    // New: Fail with HTTP status code
    public static IResult Fail(string message, HttpStatusCode statusCode) =>
        new Result { Succeeded = false, Messages = [message], StatusCode = statusCode };

    public static IResult Fail(HttpStatusCode statusCode) =>
        new Result { Succeeded = false, StatusCode = statusCode };

    public static Task<IResult> FailAsync() =>
        Task.FromResult(Fail());

    public static Task<IResult> FailAsync(string message) =>
        Task.FromResult(Fail(message));

    public static Task<IResult> FailAsync(List<string> messages) =>
        Task.FromResult(Fail(messages));

    public static Task<IResult> FailAsync(string message, HttpStatusCode statusCode) =>
        Task.FromResult(Fail(message, statusCode));

    public static IResult Success() =>
        new Result { Succeeded = true };

    public static IResult Success(string message) =>
        new Result { Succeeded = true, Messages = [message] };

    // New: Success with HTTP status code
    public static IResult Success(HttpStatusCode statusCode) =>
        new Result { Succeeded = true, StatusCode = statusCode };

    public static IResult Success(string message, HttpStatusCode statusCode) =>
        new Result { Succeeded = true, Messages = [message], StatusCode = statusCode };

    public static Task<IResult> SuccessAsync() =>
        Task.FromResult(Success());

    public static Task<IResult> SuccessAsync(string message) =>
        Task.FromResult(Success(message));

    public static Task<IResult> SuccessAsync(HttpStatusCode statusCode) =>
        Task.FromResult(Success(statusCode));

    // Convenience factory methods for common HTTP scenarios
    public static IResult Unauthorized(string? message = null) =>
        new Result { Succeeded = false, Messages = message is not null ? [message] : ["Unauthorized"], StatusCode = HttpStatusCode.Unauthorized };

    public static IResult NotFound(string? message = null) =>
        new Result { Succeeded = false, Messages = message is not null ? [message] : ["Not found"], StatusCode = HttpStatusCode.NotFound };

    public static IResult BadRequest(string? message = null) =>
        new Result { Succeeded = false, Messages = message is not null ? [message] : ["Bad request"], StatusCode = HttpStatusCode.BadRequest };

    public static IResult NoContent() =>
        new Result { Succeeded = true, StatusCode = HttpStatusCode.NoContent };

    public static IResult Cancelled(string? message = null) =>
        new Result { Succeeded = false, Messages = message is not null ? [message] : ["Operation cancelled"] };

    public static IResult ServiceUnavailable(string? message = null) =>
        new Result { Succeeded = false, Messages = message is not null ? [message] : ["Service unavailable"], StatusCode = HttpStatusCode.ServiceUnavailable };
}
