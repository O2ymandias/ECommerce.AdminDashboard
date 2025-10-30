using Microsoft.Extensions.Localization;

namespace ECommerce.APIs.Utilities.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
	public IStringLocalizer Create(Type resourceSource)
	{
		return new JsonStringLocalizer();
	}

	public IStringLocalizer Create(string baseName, string location)
	{
		return new JsonStringLocalizer();
	}
}
