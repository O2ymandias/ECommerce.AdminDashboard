using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos.UsersDtos;
using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Repositories.GenericRepo;

public class IdentityRepository(AppDbContext dbContext, UserManager<AppUser> userManager) : IIdentityRepository
{
    public async Task<IReadOnlyList<AppUserResult>> GetUsersWithRolesAsync(UserSpecsParams specsParams)
    {
        var query = dbContext.Set<AppUser>().AsQueryable();

        // Filter by search term
        if (!string.IsNullOrEmpty(specsParams.Search))
        {
            var normalizedSearch = specsParams.Search
                .Normalize()
                .ToUpperInvariant();

            query = query.Where(u =>
                u.Id == specsParams.Search ||
                (!string.IsNullOrEmpty(u.NormalizedEmail) && u.NormalizedEmail.Contains(normalizedSearch)) ||
                (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(normalizedSearch)) ||
                (!string.IsNullOrEmpty(u.NormalizedUserName) && u.NormalizedUserName.Contains(normalizedSearch))
            );
        }

        // Filter by role Id if specified
        if (!string.IsNullOrEmpty(specsParams.RoleId))
        {
            var usersInRole = dbContext.Set<IdentityUserRole<string>>()
                .Where(ur => ur.RoleId == specsParams.RoleId)
                .Select(ur => ur.UserId);

            query = query.Where(u => usersInRole.Contains(u.Id));
        }

        // Apply Pagination and Ordering (by Id for consistency)
        query = query
            .OrderBy(u => u.Id)
            .Skip((specsParams.PageNumber - 1) * specsParams.PageSize)
            .Take(specsParams.PageSize);

        // Select Specific Fields to Optimize Data Transfer
        var users = await query
            .Select(user => new AppUserResult()
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                PictureUrl = user.PictureUrl ?? string.Empty,
                Roles = Array.Empty<string>()
            })
            .ToListAsync();

        if (users.Count == 0) return [];

        // Extract user Ids from paginated users
        var userIds = users
            .Select(u => u.Id)
            .ToList();

        // Fetch users roles
        var userRolesDict = await dbContext.Set<IdentityUserRole<string>>()
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(
                dbContext.Set<IdentityRole>(),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => new { ur.UserId, RoleName = r.Name ?? string.Empty }
            )
            .GroupBy(x => x.UserId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(x => x.RoleName).ToArray()
            );

        // Assign roles
        foreach (var user in users)
        {
            if (userRolesDict.TryGetValue(user.Id, out var roles))
            {
                user.Roles = roles;
            }
        }

        return users;
    }

    public Task<int> CountAsync(UserSpecsParams specsParams)
    {
        var usersDbSet = dbContext.Set<AppUser>();

        var usersQuery = usersDbSet.AsQueryable();

        if (!string.IsNullOrEmpty(specsParams.Search))
        {
            var search = specsParams.Search.Trim().ToLower();
            usersQuery = usersQuery.Where(u =>
                (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search))
            );
        }

        return usersQuery.CountAsync();
    }
}