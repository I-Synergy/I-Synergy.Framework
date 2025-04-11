namespace ISynergy.Framework.Core.Models.Results;

public sealed class ImageResult : Result
{
    public byte[]? FileBytes { get; }
    public string? ContentType { get; }

    public ImageResult()
        : base()
    {
    }

    public ImageResult(byte[] fileBytes, string contentType)
        : this()
    {
        FileBytes = fileBytes;
        ContentType = contentType;
    }

    public new static ImageResult Fail() => new() { Succeeded = false };

    public new static ImageResult Fail(string message) => new() { Succeeded = false, Messages = [message] };

    public new static ImageResult Fail(List<string> messages) => new() { Succeeded = false, Messages = messages };

    public new static Task<ImageResult> FailAsync() => Task.FromResult(Fail());

    public new static Task<ImageResult> FailAsync(string message) => Task.FromResult(Fail(message));

    public new static Task<ImageResult> FailAsync(List<string> messages) => Task.FromResult(Fail(messages));

    public new static ImageResult Success() => new() { Succeeded = true };

    public new static ImageResult Success(string message) => new() { Succeeded = true, Messages = [message] };

    public static ImageResult Success(byte[] fileBytes, string contentType) => new(fileBytes, contentType) { Succeeded = true };

    public static ImageResult Success(byte[] fileBytes, string contentType, string message) => new(fileBytes, contentType) { Succeeded = true, Messages = [message] };

    public static ImageResult Success(byte[] fileBytes, string contentType, List<string> messages) => new(fileBytes, contentType) { Succeeded = true, Messages = messages };

    public new static Task<ImageResult> SuccessAsync() => Task.FromResult(Success());

    public new static Task<ImageResult> SuccessAsync(string message) => Task.FromResult(Success(message));

    public static Task<ImageResult> SuccessAsync(byte[] fileBytes, string contentType) => Task.FromResult(Success(fileBytes, contentType));

    public static Task<ImageResult> SuccessAsync(byte[] fileBytes, string contentType, string message) => Task.FromResult(Success(fileBytes, contentType, message));
}
