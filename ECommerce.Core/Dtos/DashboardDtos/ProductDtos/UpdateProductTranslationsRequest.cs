using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Enums;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class UpdateProductTranslationsRequest
{
    [Required] [Range(1, int.MaxValue)] public int ProductId { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; }

    [Required] public string Description { get; set; }

    [Required] public LanguageCode LanguageCode { get; set; }
}