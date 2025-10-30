namespace ECommerce.Core.Models.OrderModule;
public class DeliveryMethod : ModelBase
{
	public string ShortName { get; set; }
	public string Description { get; set; }
	public string DeliveryTime { get; set; }
	public decimal Cost { get; set; }
	public ICollection<DeliveryMethodTranslation> Translations { get; set; }
}
