namespace ECommerce.Core.Dtos.RatingDtos;
public class LatestRatingsResult
{
	public int Count { get; set; }
	public IReadOnlyList<RatingResult> LatestRatings { get; set; }
}
