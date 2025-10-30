using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.UsersDtos;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<IReadOnlyList<AppUserResult>> GetUsersWithRolesAsync(UserSpecsParams specsParams);
    Task<int> CountAsync(UserSpecsParams specsParams);
}