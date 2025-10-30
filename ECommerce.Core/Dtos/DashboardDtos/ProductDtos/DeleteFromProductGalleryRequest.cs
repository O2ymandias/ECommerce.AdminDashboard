using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class DeleteFromProductGalleryRequest
{
    [Required] [Range(1, int.MaxValue)] public int ProductId { get; set; }

    [Required] public string ImagePath { get; set; }
}