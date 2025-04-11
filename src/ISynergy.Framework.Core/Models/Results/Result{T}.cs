using ISynergy.Framework.Core.Abstractions;

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

    public new static Result<T> Fail() => new() { Succeeded = false };

    public new static Result<T> Fail(string message) => new() { Succeeded = false, Messages = [message] };

    public new static Result<T> Fail(List<string> messages) => new() { Succeeded = false, Messages = messages };

    public new static Task<Result<T>> FailAsync() => Task.FromResult(Fail());

    public new static Task<Result<T>> FailAsync(string message) => Task.FromResult(Fail(message));

    public new static Task<Result<T>> FailAsync(List<string> messages) => Task.FromResult(Fail(messages));

    public new static Result<T> Success() => new() { Succeeded = true };

    public new static Result<T> Success(string message) => new() { Succeeded = true, Messages = [message] };

    public static Result<T> Success(T data) => new(data) { Succeeded = true };

    public static Result<T> Success(T data, string message) => new(data) { Succeeded = true, Messages = [message] };

    public static Result<T> Success(T data, List<string> messages) => new(data) { Succeeded = true, Messages = messages };

    public new static Task<Result<T>> SuccessAsync() => Task.FromResult(Success());

    public new static Task<Result<T>> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));

    public static Task<Result<T>> SuccessAsync(T data, string message) => Task.FromResult(Success(data, message));
}
