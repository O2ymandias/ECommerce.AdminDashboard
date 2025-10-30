using ECommerce.Core.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        [HttpGet("order-status")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetOrderStatus() =>
            Ok(Enum.GetNames<OrderStatus>());

        [HttpGet("payment-status")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetPaymentStatus() =>
            Ok(Enum.GetNames<PaymentStatus>());

        [HttpGet("payment-method")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetPaymentMethod() =>
            Ok(Enum.GetNames<PaymentMethod>());
    }
}