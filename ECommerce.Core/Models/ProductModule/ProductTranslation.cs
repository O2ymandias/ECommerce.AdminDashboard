using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Models.ProductModule;

public class ProductTranslation : ModelBase
{
    public LanguageCode LanguageCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}
