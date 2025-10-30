namespace ECommerce.Core.Models.OrderModule.Owned;
public class ProductItem
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string PictureUrl { get; set; }

	public IDictionary<string, string> NameTranslations { get; set; }
}
