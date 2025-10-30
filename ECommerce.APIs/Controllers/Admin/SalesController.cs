using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.OrdersDto;
using ECommerce.Core.Dtos.DashboardDtos.SalesDto;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers.Admin
{
    public class SalesController(ISalesService salesService) : AdminController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<SalesResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SalesResult>>> GetSales(
            [FromQuery] SalesSpecsParams specsParams)
        {
            return Ok(await salesService.GetSalesAsync(specsParams));
        }

        [HttpGet("total")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalSales(
            [FromQuery] RevenueRequest request)
        {
            return Ok(await salesService.GetRevenueAsync(request));
        }
    }
}