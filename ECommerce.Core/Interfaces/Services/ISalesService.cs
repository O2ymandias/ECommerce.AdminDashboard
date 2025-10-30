using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;
using ECommerce.Core.Dtos.DashboardDtos.SalesDto;

namespace ECommerce.Core.Interfaces.Services;

public interface ISalesService
{
    Task<PaginationResult<SalesResult>> GetSalesAsync(SalesSpecsParams SpecsParams);
    Task<decimal> GetRevenueAsync(RevenueRequest request);
}