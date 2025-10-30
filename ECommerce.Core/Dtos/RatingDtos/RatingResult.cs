namespace ECommerce.Core.Dtos.RatingDtos;
public class RatingResult
{
	public int Id { get; set; }
	public int Stars { get; set; }
	public string? Title { get; set; }
	public string? Comment { get; set; }
	public DateTime CreatedAt { get; set; }
	public RatedProduct Product { get; set; }
	public RatingUser User { get; set; }
}
