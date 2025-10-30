namespace ECommerce.Core.Common.SpecsParams;
public class RatingSpecsParams : BaseSpecsParams
{
	public int? RatingId { get; set; }
	public int? ProductId { get; set; }
	public string? UserId { get; set; }
}
