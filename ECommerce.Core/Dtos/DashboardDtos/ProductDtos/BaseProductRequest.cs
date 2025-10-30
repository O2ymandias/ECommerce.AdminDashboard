using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class BaseProductRequest
{
    [Required] [MaxLength(100)] public string Name { get; set; }

    [Required] public string Description { get; set; }

    [Required] [Range(1, int.MaxValue)] public decimal Price { get; set; }

    [Required] [Range(0, int.MaxValue)] public int UnitsInStock { get; set; }

    [Required] [Range(1, int.MaxValue)] public int BrandId { get; set; }

    [Required] [Range(1, int.MaxValue)] public int CategoryId { get; set; }
}