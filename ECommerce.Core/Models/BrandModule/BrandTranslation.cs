using ECommerce.Core.Common.Enums;
namespace ECommerce.Core.Models.BrandModule;
public class BrandTranslation : ModelBase
{
	public LanguageCode LanguageCode { get; set; }
	public string Name { get; set; }
	public Brand Brand { get; set; }
	public int BrandId { get; set; }
}
