namespace ECommerce.Core.Models.BrandModule;

public class Brand : ModelBase
{
	public string Name { get; set; }
	public string? PictureUrl { get; set; }
	public ICollection<BrandTranslation> Translations { get; set; } = [];
}
