using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Dtos.DashboardDtos.BrandOrCategoryDtos;

public class UpdateBrandOrCategoryRequest
{
    [Required] [Range(1, int.MaxValue)] public int Id { get; set; }

    [Required] [MaxLength(50)] public string Name { get; set; }

    public IFormFile? Image { get; set; }

    [Required] public List<UpdateBrandOrCategoryTranslations> Translations { get; set; }
}