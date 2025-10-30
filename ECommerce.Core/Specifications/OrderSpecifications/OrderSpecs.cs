using ECommerce.Core.Common;
using ECommerce.Core.Common.Enums;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Models.OrderModule;
using System.Linq.Expressions;

namespace ECommerce.Core.Specifications.OrderSpecifications;

public class OrderSpecs : BaseSpecification<Order>
{
    private static readonly Expression<Func<Order, object>> DefaultSort = p => p.OrderDate;

    public OrderSpecs(
        OrderSpecsParams specsParams,
        bool enablePagination = true,
        bool enableSorting = true,
        bool enableTracking = true
    )
    {
        ApplyFiltration(specsParams);
        if (enablePagination) ApplyPagination(specsParams.PageNumber, specsParams.PageSize);
        if (enableSorting) SortBy(specsParams.Sort);
        if (enableTracking) IsTrackingEnabled = true;
    }

    protected void SortBy(OrderSortOptions sortOptions)
    {
        switch (sortOptions.Dir)
        {
            case SortDirection.Asc:
                SortAsc = sortOptions.Key switch
                {
                    OrderSortKey.CreatedAt => o => o.OrderDate,
                    OrderSortKey.SubTotal => o => o.SubTotal,
                    _ => DefaultSort
                };
                break;

            case SortDirection.Desc:
                SortDesc = sortOptions.Key switch
                {
                    OrderSortKey.CreatedAt => o => o.OrderDate,
                    OrderSortKey.SubTotal => o => o.SubTotal,
                    _ => DefaultSort
                };
                break;

            default:
                SortDesc = DefaultSort;
                break;
        }
    }

    protected void ApplyFiltration(OrderSpecsParams specsParams)
    {
        Criteria = order =>
            // Order ID filter
            (!specsParams.OrderId.HasValue || order.Id == specsParams.OrderId.Value) &&

            // Status filters
            (!specsParams.OrderStatus.HasValue || order.OrderStatus == specsParams.OrderStatus) &&
            (!specsParams.PaymentStatus.HasValue || order.PaymentStatus == specsParams.PaymentStatus) &&

            // Payment method filter
            (!specsParams.PaymentMethod.HasValue || order.PaymentMethod == specsParams.PaymentMethod) &&

            // Subtotal range
            (!specsParams.MinSubtotal.HasValue || order.SubTotal >= specsParams.MinSubtotal) &&
            (!specsParams.MaxSubtotal.HasValue || order.SubTotal <= specsParams.MaxSubtotal) &&

            // Date range
            (!specsParams.StartDate.HasValue || order.OrderDate >= specsParams.StartDate) &&
            (!specsParams.EndDate.HasValue || order.OrderDate <= specsParams.EndDate) &&

            // User ID filter
            (string.IsNullOrWhiteSpace(specsParams.UserId) || order.UserId.Equals(specsParams.UserId)) &&

            // Search filter
            (string.IsNullOrWhiteSpace(specsParams.Search) || order.Id.ToString().Equals(specsParams.Search));
    }
}