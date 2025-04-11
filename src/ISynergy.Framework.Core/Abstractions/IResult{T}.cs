namespace ISynergy.Framework.Core.Abstractions;

public interface IResult<out T> : IResult
{
    T? Data { get; }
}
