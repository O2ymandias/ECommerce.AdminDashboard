namespace ECommerce.Core.Common.Options;
public class JwtOptions
{
	public string Issuer { get; set; }
	public string Audience { get; set; }
	public int MinutesToExpire { get; set; }
	public string SecurityKey { get; set; }
}
