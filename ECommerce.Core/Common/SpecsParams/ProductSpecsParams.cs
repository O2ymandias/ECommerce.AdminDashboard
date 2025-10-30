using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Common.Constants;

namespace ECommerce.Core.Common.SpecsParams;

public class ProductSpecsParams : BaseSpecsParams
{
    public string? Search { get; set; }
    [Range(1, int.MaxValue)] public int? ProductId { get; set; }
    [Range(1, int.MaxValue)] public int? CategoryId { get; set; }
    [Range(1, int.MaxValue)] public int? BrandId { get; set; }
    public ProductSortOptions Sort { get; set; } = new();
    [Range(1, int.MaxValue)] public int? MinPrice { get; set; }
    [Range(1, int.MaxValue)] public int? MaxPrice { get; set; }
}