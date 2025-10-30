using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;

public class CreateBrandOrCategoryRequest
{
    [Required] [MaxLength(50)] public string Name { get; set; }

    [Required] public IFormFile Image { get; set; }

    [Required] public List<UpdateBrandOrCategoryTranslations> Translations { get; set; }
}