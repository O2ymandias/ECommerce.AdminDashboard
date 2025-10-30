using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.UsersDtos;
using ECommerce.Core.Dtos.ProfileDtos;
using ECommerce.Core.Models.AuthModule;

namespace ECommerce.Core.Interfaces.Services;

public interface IUserService
{
    Task<PaginationResult<AppUserResult>> GetUsersAsync(UserSpecsParams specsParams);
    Task<AppUserResult?> GetUserAsync(string userId);
    Task<SaveResult> AssignUserToRolesAsync(AssignToRoleRequest requestData);
    Task<int> GetUsersCountAsync();
}