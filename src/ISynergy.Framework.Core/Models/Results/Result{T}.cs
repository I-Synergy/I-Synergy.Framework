using ISynergy.Framework.Core.Abstractions;
using System.Net;

namespace ISynergy.Framework.Core.Models.Results;

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; }

    public Result()
        : base()
    {
    }

    public Result(T data)
        : this()
    {
        Data = data;
    }

    // Existing methods
    public new static Result<T> Fail() =>
        new() { Succeeded = false };

    public new static Result<T> Fail(string message) =>
        new() { Succeeded = false, Messages = [message] };

    public new static Result<T> Fail(List<string> messages) =>
        new() { Succeeded = false, Messages = messages };

    // New: Fail with HTTP status code
    public new static Result<T> Fail(string message, HttpStatusCode statusCode) =>
        new() { Succeeded = false, Messages = [message], StatusCode = statusCode };

    public new static Result<T> Fail(HttpStatusCode statusCode) =>
        new() { Succeeded = false, StatusCode = statusCode };

    public new static Task<Result<T>> FailAsync() =>
        Task.FromResult(Fail());

    public new static Task<Result<T>> FailAsync(string message) =>
        Task.FromResult(Fail(message));

    public new static Task<Result<T>> FailAsync(List<string> messages) =>
        Task.FromResult(Fail(messages));

    public new static Task<Result<T>> FailAsync(string message, HttpStatusCode statusCode) =>
        Task.FromResult(Fail(message, statusCode));

    // Note: Success(string message) is intentionally NOT overridden here to avoid ambiguity when T is string.
    // Use Result<T>.Success(data) for data, or cast to base Result for message-only success.

    public new static Result<T> Success() =>
        new() { Succeeded = true };

    public static Result<T> Success(T data) =>
        new(data) { Succeeded = true };

    public static Result<T> Success(T data, string message) =>
        new(data) { Succeeded = true, Messages = [message] };

    public static Result<T> Success(T data, List<string> messages) =>
        new(data) { Succeeded = true, Messages = messages };

    // Success with data and HTTP status code
    public static Result<T> Success(T data, HttpStatusCode statusCode) =>
        new(data) { Succeeded = true, StatusCode = statusCode };

    public static Result<T> Success(T data, string message, HttpStatusCode statusCode) =>
        new(data) { Succeeded = true, Messages = [message], StatusCode = statusCode };

    public new static Task<Result<T>> SuccessAsync() =>
        Task.FromResult(Success());

    public static Task<Result<T>> SuccessAsync(T data) =>
        Task.FromResult(Success(data));

    public static Task<Result<T>> SuccessAsync(T data, string message) =>
        Task.FromResult(Success(data, message));

    public static Task<Result<T>> SuccessAsync(T data, HttpStatusCode statusCode) =>
        Task.FromResult(Success(data, statusCode));

    // Convenience factory methods for common HTTP scenarios
    public new static Result<T> Unauthorized(string? message = null) =>
        new() { Succeeded = false, Messages = message is not null ? [message] : ["Unauthorized"], StatusCode = HttpStatusCode.Unauthorized };

    public new static Result<T> NotFound(string? message = null) =>
        new() { Succeeded = false, Messages = message is not null ? [message] : ["Not found"], StatusCode = HttpStatusCode.NotFound };

    public new static Result<T> BadRequest(string? message = null) =>
        new() { Succeeded = false, Messages = message is not null ? [message] : ["Bad request"], StatusCode = HttpStatusCode.BadRequest };

    public new static Result<T> NoContent() =>
        new() { Succeeded = true, StatusCode = HttpStatusCode.NoContent };

    public new static Result<T> Cancelled(string? message = null) =>
        new() { Succeeded = false, Messages = message is not null ? [message] : ["Operation cancelled"] };

    public new static Result<T> ServiceUnavailable(string? message = null) =>
        new() { Succeeded = false, Messages = message is not null ? [message] : ["Service unavailable"], StatusCode = HttpStatusCode.ServiceUnavailable };
}