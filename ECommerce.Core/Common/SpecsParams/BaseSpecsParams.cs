using ECommerce.Core.Common.Constants;

namespace ECommerce.Core.Common.SpecsParams;

public class BaseSpecsParams
{
    private int _pageNumber = PaginationConstants.DefaultPageIndex;
    private int _pageSize = PaginationConstants.DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value > 0
            ? value
            : PaginationConstants.DefaultPageIndex;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value switch
            {
                > PaginationConstants.MaxPageSize => PaginationConstants.MaxPageSize,
                <= 0 => PaginationConstants.DefaultPageSize,
                _ => value
            };
        }
    }
}