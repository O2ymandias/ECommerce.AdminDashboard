using ECommerce.Core.Common.Enums;
using ECommerce.Core.Interfaces.Services;
using System.Globalization;

namespace ECommerce.Application.Services;

public class CultureService : ICultureService
{
	private readonly LanguageCode _defaultLanguageCode = LanguageCode.EN;

	public CultureService()
	{
		var culture = Thread.CurrentThread.CurrentCulture;
		var canParse = Enum.TryParse(culture.Name, ignoreCase: true, out LanguageCode lang);

		Culture = culture;
		LanguageCode = canParse ? lang : _defaultLanguageCode;
		CanTranslate = canParse && lang != _defaultLanguageCode;
	}

	public CultureInfo Culture { get; private set; }
	public LanguageCode LanguageCode { get; private set; }
	public bool CanTranslate { get; private set; }

}
