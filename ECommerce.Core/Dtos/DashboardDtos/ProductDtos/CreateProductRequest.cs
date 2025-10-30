using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Dtos.DashboardDtos.ProductDtos;

public class CreateProductRequest : BaseProductRequest
{
    [Required] public IFormFile Image { get; set; }
}