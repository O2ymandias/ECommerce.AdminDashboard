using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.DashboardDtos;
using ECommerce.Core.Dtos.DashboardDtos.UsersDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Services;

public class UserService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IUnitOfWork unitOfWork,
    IConfiguration config)
    : IUserService
{
    public async Task<PaginationResult<AppUserResult>> GetUsersAsync(UserSpecsParams specsParams)
    {
        var users = await unitOfWork.IdentityRepository.GetUsersWithRolesAsync(specsParams);
        var total = await unitOfWork.IdentityRepository.CountAsync(specsParams);
        return new PaginationResult<AppUserResult>()
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Total = total,
            Results =
            [
                .. users.Select(user =>
                {
                    user.PictureUrl = user.PictureUrl is not null
                        ? $"{config["BaseUrl"]}/{user.PictureUrl}"
                        : null;
                    return user;
                })
            ]
        };
    }

    public async Task<AppUserResult?> GetUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return new AppUserResult()
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,
            PictureUrl = user.PictureUrl is not null
                ? $"{config["BaseUrl"]}/{user.PictureUrl}"
                : null,
            Roles = [.. roles]
        };
    }

    public async Task<SaveResult> AssignUserToRolesAsync(AssignToRoleRequest requestData)
    {
        var user = await userManager.FindByIdAsync(requestData.UserId);
        if (user is null)
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found."
            };

        // Get App Roles from Constants
        var roles = await roleManager.Roles
            .Select(role => role.Name)
            .ToListAsync();

        foreach (var role in requestData.Roles)
        {
            if (!roles.Contains(role))
            {
                return new SaveResult()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Invalid role: {role}"
                };
            }
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = requestData.Roles.Except(currentRoles).ToArray();
        var rolesToRemove = currentRoles.Except(requestData.Roles).ToArray();

        var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
        if (!addResult.Succeeded)
        {
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to add roles: {string.Join(", ", addResult.Errors.Select(e => e.Description))}"
            };
        }

        var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (!removeResult.Succeeded)
        {
            return new SaveResult()
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Failed to remove roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}"
            };
        }

        return new SaveResult()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "User roles updated successfully."
        };
    }

    public async Task<int> GetUsersCountAsync() =>
        await userManager.Users.CountAsync();
}