using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class AddToProductGalleryRequest
{
    [Required] public IFormFile[] Images { get; set; }

    [Required] [Range(1, int.MaxValue)] public int ProductId { get; set; }
}