using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECommerce.APIs.Utilities.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
	public LocalizedString this[string name]
	{
		get
		{
			var value = GetString(name);
			return new LocalizedString(name, value);
		}
	}

	public LocalizedString this[string name, params object[] arguments]
	{
		get
		{
			var actualValue = this[name];
			return actualValue.ResourceNotFound
				? actualValue
				: new LocalizedString(name, string.Format(actualValue, arguments));
		}
	}

	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		var filePath = GetResourceFilePath();
		var fullFilePath = Path.GetFullPath(filePath);

		if (!File.Exists(fullFilePath))
			yield break;

		JObject jsonObject;
		try
		{
			using var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var streamReader = new StreamReader(fileStream);
			using var reader = new JsonTextReader(streamReader);

			jsonObject = JObject.Load(reader);

			if (jsonObject is null)
				yield break;
		}
		catch (Exception)
		{
			yield break;
		}

		foreach (var property in jsonObject.Properties())
		{
			var value = property.Value.ToString();
			yield return new LocalizedString(property.Name, value, false);
		}
	}


	private static string GetString(string key)
	{
		var filePath = GetResourceFilePath();
		var fullFilePath = Path.GetFullPath(filePath);

		if (File.Exists(fullFilePath))
			return GetValueFromJson(key, fullFilePath);

		return string.Empty;
	}

	private static string GetValueFromJson(string propertyName, string filePath)
	{
		if (string.IsNullOrEmpty(propertyName))
			throw new ArgumentException("Property name cannot be null or empty", nameof(propertyName));

		if (string.IsNullOrEmpty(filePath))
			throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

		try
		{
			using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var streamReader = new StreamReader(fileStream);
			using var reader = new JsonTextReader(streamReader);

			#region Root Properties Only
			//while (reader.Read())
			//	if (reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value as string, propertyName))
			//	{
			//		reader.Read(); // Move to the next Token (Actual Value)
			//		return _jsonSerializer.Deserialize<string>(reader) ?? string.Empty;
			//	}
			//return string.Empty; 
			#endregion

			#region Nested Properties
			JObject jsonObject = JObject.Load(reader);
			var token = jsonObject.SelectToken(propertyName);
			if (token is null) return string.Empty;
			return token.ToString();
			#endregion
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	private static string GetResourceFilePath() =>
		$"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
}