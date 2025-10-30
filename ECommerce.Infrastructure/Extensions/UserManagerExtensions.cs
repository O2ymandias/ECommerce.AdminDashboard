using ECommerce.Core.Models.AuthModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Infrastructure.Extensions;

public static class UserManagerExtensions
{
	public static async Task<AppUser?> FindByEmailWithIncludesAsync(
		this UserManager<AppUser> userManager,
		string email,
		params Expression<Func<AppUser, object?>>[] includes
		)
	{
		var query = userManager.Users.Where(u => u.NormalizedEmail == userManager.NormalizeEmail(email));
		query = includes.Aggregate(query, (acc, next) => acc.Include(next));
		return await query.FirstOrDefaultAsync();
	}

	public static async Task<AppUser?> FindByNameWithIncludesAsync(
		this UserManager<AppUser> userManager,
		string userName,
		params Expression<Func<AppUser, object?>>[] includes
		)
	{
		var query = userManager.Users.Where(u => u.NormalizedUserName == userManager.NormalizeName(userName));
		query = includes.Aggregate(query, (curr, next) => curr.Include(next));
		return await query.FirstOrDefaultAsync();
	}
	public static async Task<AppUser?> FindByIdWithIncludesAsync(
		this UserManager<AppUser> userManager,
		string userId,
		params Expression<Func<AppUser, object?>>[] includes
		)
	{
		var query = userManager.Users.Where(u => u.Id == userId);
		query = includes.Aggregate(query, (curr, next) => curr.Include(next));
		return await query.FirstOrDefaultAsync();
	}


}
