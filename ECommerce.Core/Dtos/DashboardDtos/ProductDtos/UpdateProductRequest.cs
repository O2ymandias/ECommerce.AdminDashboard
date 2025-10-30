using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class UpdateProductRequest : BaseProductRequest
{
    [Required] public int Id { get; set; }

    public IFormFile? Image { get; set; }
}