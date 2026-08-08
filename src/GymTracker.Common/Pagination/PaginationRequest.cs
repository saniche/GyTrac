using System;

namespace GymTracker.Common.Pagination;

public sealed record PaginationRequest(SearchValue SearchValue, QueryPage PageIndex, QueryPageSize Size)
{
    public int Page { get; } = PageIndex.Value;

    //max value of 100 for page size to prevent abuse
    public int PageSize { get; } = Size.Value;
}