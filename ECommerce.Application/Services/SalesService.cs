using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;
using ECommerce.Core.Dtos.DashboardDtos.SalesDto;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.OrderModule;
using ECommerce.Core.Specifications.OrderSpecifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class SalesService(IUnitOfWork unitOfWork) : ISalesService
{
    public async Task<PaginationResult<SalesResult>> GetSalesAsync(SalesSpecsParams specsParams)
    {
        var query = unitOfWork
            .Repository<OrderItem>()
            .GetAllAsQueryable(null);

        // Filter by successful Sales (OrderStatus.Delivered && PaymentStatus.PaymentReceived) 
        query = query.Where(oi =>
            oi.Order.OrderStatus == OrderStatus.Delivered &&
            oi.Order.PaymentStatus == PaymentStatus.PaymentReceived
        );


        // Group sales data by product
        var groupedQuery = query
            .GroupBy(oi => oi.Product.Id)
            .Select(g => new SalesResult
            {
                ProductId = g.Key,
                ProductName = g.First().Product.Name,
                ProductPictureUrl = g.First().Product.PictureUrl,
                UnitsSold = g.Sum(oi => oi.Quantity),
                TotalSales = g.Sum(oi => oi.Quantity * oi.Price),
            });

        // Total count before pagination
        var totalCount = await groupedQuery.CountAsync();

        // Apply sorting
        var isSortDesc = specsParams.Sort.Dir == SortDirection.Desc;
        groupedQuery = specsParams.Sort.Key switch
        {
            SalesSortKey.TotalSales => isSortDesc
                ? groupedQuery.OrderByDescending(s => s.TotalSales)
                : groupedQuery.OrderBy(s => s.TotalSales),

            SalesSortKey.UnitsSold => isSortDesc
                ? groupedQuery.OrderByDescending(s => s.UnitsSold)
                : groupedQuery.OrderBy(s => s.UnitsSold),

            _ => groupedQuery.OrderByDescending(s => s.UnitsSold)
        };

        // Pagination
        var items = await groupedQuery
            .Skip((specsParams.PageNumber - 1) * specsParams.PageSize)
            .Take(specsParams.PageSize)
            .ToListAsync();

        // Return paginated result
        return new PaginationResult<SalesResult>
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Total = totalCount,
            Results = items
        };
    }

    public async Task<decimal> GetRevenueAsync(RevenueRequest request)
    {
        var orderSpecs = new OrderSpecs(
            new OrderSpecsParams
            {
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                OrderStatus = OrderStatus.Delivered,
                PaymentStatus = PaymentStatus.PaymentReceived,
            },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false
        );

        orderSpecs.IncludeRelatedData(o => o.DeliveryMethod);

        var orders = await unitOfWork
            .Repository<Order>()
            .GetAllAsync(orderSpecs);

        return orders.Sum(o => o.Total);
    }
}