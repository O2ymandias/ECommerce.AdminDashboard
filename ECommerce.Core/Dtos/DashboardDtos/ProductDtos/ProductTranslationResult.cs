using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class ProductTranslationResult
{
    public LanguageCode LanguageCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}