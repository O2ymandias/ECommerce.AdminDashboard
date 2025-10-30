using ECommerce.Core.Common.Enums;
namespace ECommerce.Core.Models.CategoryModule;
public class CategoryTranslation : ModelBase
{
	public LanguageCode LanguageCode { get; set; }
	public string Name { get; set; }
	public Category Category { get; set; }
	public int CategoryId { get; set; }
}
