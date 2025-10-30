using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;

public class UpdateBrandOrCategoryTranslations
{
    [Required] public string Name { get; set; }

    [Required] public LanguageCode LanguageCode { get; set; }
}