namespace ECommerce.Core.Common.Pagination;
public class PaginationResult<T> where T : class
{
	public int PageSize { get; set; }
	public int PageNumber { get; set; }
	public int Total { get; set; }
	public int Pages => (int)Math.Ceiling(Total / (double)PageSize);
	public bool HasNext => PageNumber < Pages;
	public bool HasPrevious => PageNumber > 1;
	public required IReadOnlyList<T> Results { get; set; }

}
