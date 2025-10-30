using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core.Common.Helpers;
public static class IdentityResultLogger
{
	public static void LogInfo(IdentityResult result, ILogger logger, string description)
	{
		if (result.Succeeded)
			logger.LogInformation("Success: {Description}", description);
		else
			logger.LogError("Fail: {Description}", string.Join(" ", result.Errors.Select(e => e.Description)));
	}

	public static void LogWarning(IdentityResult result, ILogger logger, string description)
	{
		if (result.Succeeded)
			logger.LogWarning("Success: {Description}", description);
		else
			logger.LogError("Fail: {Description}", string.Join(" ", result.Errors.Select(e => e.Description)));
	}
}
