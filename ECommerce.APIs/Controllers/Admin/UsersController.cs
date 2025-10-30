using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.UsersDtos;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ECommerce.APIs.Controllers.Admin
{
    public class UsersController(IUserService userService, IOrderService orderService, IApiErrorResponseFactory error)
        : AdminController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginationResult<AppUserResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginationResult<AppUserResult>>> GetUsers(
            [FromQuery] UserSpecsParams specsParams)
        {
            var users = await userService.GetUsersAsync(specsParams);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppUserResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserResult>> GetUser(string id)
        {
            var user = await userService.GetUserAsync(id);
            return user is null
                ? NotFound(error.CreateErrorResponse(StatusCodes.Status404NotFound))
                : Ok(user);
        }

        [HttpGet("count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<AppUserResult>> GetUsersCount() =>
            Ok(await userService.GetUsersCountAsync());

        [HttpPut("assign-roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignUserToRoles([FromBody] AssignToRoleRequest requestData)
        {
            var result = await userService.AssignUserToRolesAsync(requestData);
            return result.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(result),
                StatusCodes.Status400BadRequest => BadRequest(result),
                StatusCodes.Status404NotFound => NotFound(result),
                _ => StatusCode(result.StatusCode)
            };
        }
    }
}