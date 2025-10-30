namespace ECommerce.Core.Models.CategoryModule;
public class Category : ModelBase
{
	public string Name { get; set; }
	public string? PictureUrl { get; set; }
	public ICollection<CategoryTranslation> Translations { get; set; }
}
