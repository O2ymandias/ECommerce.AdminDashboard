using ECommerce.Core.Models;
using System.Linq.Expressions;

namespace ECommerce.Core.Interfaces;
public interface ISpecification<TEntity> where TEntity : ModelBase
{
	Expression<Func<TEntity, bool>>? Criteria { get; set; }
	List<Expression<Func<TEntity, object>>> Includes { get; set; }
	//public List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> IncludesWithThen { get; set; }
	Expression<Func<TEntity, object>>? SortAsc { get; set; }
	Expression<Func<TEntity, object>>? SortDesc { get; set; }
	int Take { get; set; }
	int Skip { get; set; }
	bool IsPaginationEnabled { get; set; }
	bool IsTrackingEnabled { get; set; }
	bool IsSplitQueryEnabled { get; set; }
}
