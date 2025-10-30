using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Common;
public class Email
{
	public List<string> To { get; set; }
	public string Subject { get; set; }
	public string Body { get; set; }
	public List<IFormFile>? Attachments { get; set; }
	public bool IsHtml { get; set; }
}
