using ECommerce.Core.Interfaces;
using ECommerce.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories.GenericRepo;

internal static class SpecificationsEvaluator<TEntity> where TEntity : ModelBase
{
    public static IQueryable<TEntity> BuildQuery(IQueryable<TEntity> inpQuery, ISpecification<TEntity> specs)
    {
        var query = inpQuery;

        if (specs.Criteria is not null)
            query = query.Where(specs.Criteria);

        if (specs.SortAsc is not null)
            query = query.OrderBy(specs.SortAsc);

        else if (specs.SortDesc is not null)
            query = query.OrderByDescending(specs.SortDesc);

        if (specs.IsPaginationEnabled)
            query = query.Skip(specs.Skip).Take(specs.Take);

        if (specs.Includes?.Count > 0)
            query = specs.Includes.Aggregate(query, (currQuery, nextExpr) => currQuery.Include(nextExpr));


        if (specs.IsSplitQueryEnabled)
            query = query.AsSplitQuery();

        query = specs.IsTrackingEnabled ? query.AsTracking() : query.AsNoTracking();

        return query;
    }
}