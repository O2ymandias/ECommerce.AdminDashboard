using ECommerce.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.APIs.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute<TCacheValue>(
	ICacheService cacheService,
	int minsToExpireAfter)
	: Attribute, IAsyncResourceFilter where TCacheValue : class
{
	public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
	{
		var key = GenerateCacheKey(context);
		var cachedVal = await cacheService.GetCacheAsync<TCacheValue>(key);
		if (cachedVal is not null)
		{
			context.Result = new OkObjectResult(cachedVal);
			return;
		}
		var resourceExecutedContext = await next();
		if (resourceExecutedContext.Result is OkObjectResult ok &&
			ok.Value is TCacheValue result)
		{
			await cacheService.SetCacheAsync(key, result, TimeSpan.FromMinutes(minsToExpireAfter));
		}
	}

	private static string GenerateCacheKey(ResourceExecutingContext context)
	{
		var key = new StringBuilder();

		key.Append($"{context.HttpContext.Request.Path}?");

		foreach (var q in context.HttpContext.Request.Query.OrderBy(q => q.Key))
			key.Append($"{q.Key}={q.Value}&");

		return key.ToString().TrimEnd('&', '?');
	}

}
