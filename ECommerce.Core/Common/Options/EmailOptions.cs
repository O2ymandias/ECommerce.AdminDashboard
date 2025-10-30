namespace ECommerce.Core.Common.Options;
public class EmailOptions
{
	public string SmtpHost { get; set; }
	public int SmtpPort { get; set; }
	public string DisplayName { get; set; }
	public string SenderEmail { get; set; }
	public string Password { get; set; }

	public EmailAttachmentOptions EmailAttachmentOptions { get; set; }
}


public class EmailAttachmentOptions
{
	public int MaxFileSize { get; set; }
	public string[] AllowedExtensions { get; set; }
	public int MaxFileCount { get; set; }
}
