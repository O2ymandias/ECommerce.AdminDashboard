namespace ECommerce.Core.Dtos.ProfileDtos;
public class EmailChangeCache
{
	public string UserId { get; set; }
	public string NewEmail { get; set; }
	public string VerificationCode { get; set; }
}
