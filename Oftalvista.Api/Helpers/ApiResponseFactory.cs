using Oftalvista.Api.Contracts;

namespace Oftalvista.Api.Helpers;

public static class ApiResponseFactory
{
    public static PaginatedItemsResponse<T> Page<T>(
        IReadOnlyCollection<T> data,
        int count,
        int pageSize,
        int skip
    ) =>
        new()
        {
            PageIndex = pageSize <= 0 ? 0 : skip / pageSize,
            PageSize = pageSize,
            Count = count,
            Data = data,
        };

    public static int NormalizePageSize(int pageSize) =>
        pageSize <= 0 ? 10 : Math.Min(pageSize, 500);

    public static int NormalizeSkip(int skip) => Math.Max(skip, 0);

    public static bool IsDescending(string? sortDir) =>
        string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
}
