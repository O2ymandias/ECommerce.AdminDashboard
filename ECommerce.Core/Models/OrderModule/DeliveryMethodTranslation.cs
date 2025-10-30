using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Models.OrderModule;
public class DeliveryMethodTranslation : ModelBase
{
	public LanguageCode LanguageCode { get; set; }
	public string ShortName { get; set; }
	public string Description { get; set; }
	public string DeliveryTime { get; set; }

	public DeliveryMethod DeliveryMethod { get; set; }
	public int DeliveryMethodId { get; set; }
}
