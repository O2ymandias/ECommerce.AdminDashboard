using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.RolesDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class RoleService(RoleManager<IdentityRole> roleManager) : IRoleService
{
    public async Task<IReadOnlyList<RoleResult>> GetRolesAsync()
    {
        IReadOnlyList<RoleResult> roles = await roleManager.Roles
            .Select(r => new RoleResult()
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
            })
            .ToListAsync();

        return roles;
    }
}