using ISynergy.Framework.Core.Models.Results;

namespace ISynergy.Framework.Core.Extensions;

public static class ResultExtensions
{
    public static TResult Match<T, TResult>(this Result<T> result, Func<T?, TResult> onSuccess, Func<TResult> onFailure)
    {
        return result.Succeeded ? onSuccess(result.Data) : onFailure();
    }

    public static TResult Match<T, TResult>(this PaginatedResult<T> result, Func<IEnumerable<T>?, TResult> onSuccess, Func<TResult> onFailure)
    {
        return result.Succeeded ? onSuccess(result.Data) : onFailure();
    }
}
