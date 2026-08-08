namespace GymTracker.Common.Pagination;

public class PaginatedResult<TEntity>(QueryPage page, QueryPageSize pageSize, long totalCount, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int Page { get; } = page.Value;
    public int PageSize { get; } = pageSize.Value;
    public long TotalCount { get; } = totalCount;
    public IEnumerable<TEntity> Data { get; } = data;

    //Computed properties
    public int Count => Data.Count();
    public long TotalPages => (long)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}