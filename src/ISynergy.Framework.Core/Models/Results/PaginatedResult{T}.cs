namespace ISynergy.Framework.Core.Models.Results;

public class PaginatedResult<T> : Result
{
    public IEnumerable<T>? Data { get; }

    public int CurrentPage { get; }

    public int TotalPages { get; }

    public int TotalCount { get; }

    public int PageSize { get; }

    public PaginatedResult(IEnumerable<T> data)
        : base()
    {
        Data = data;
    }

    internal PaginatedResult(bool succeeded, IEnumerable<T>? data = default, List<string>? messages = null, int count = 0, int page = 1, int pageSize = 10)
    {
        Data = data;
        CurrentPage = page;
        Succeeded = succeeded;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }

    public static PaginatedResult<T> Failure(List<string> messages) => new PaginatedResult<T>(false, default, messages);

    public static PaginatedResult<T> Success(IEnumerable<T> data, int count, int page, int pageSize) => new PaginatedResult<T>(true, data, null, count, page, pageSize);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}
