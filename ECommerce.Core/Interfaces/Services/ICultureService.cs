using ECommerce.Core.Common.Enums;
using System.Globalization;

namespace ECommerce.Core.Interfaces.Services;
public interface ICultureService
{
	public CultureInfo Culture { get; }
	public LanguageCode LanguageCode { get; }
	public bool CanTranslate { get; }
}
