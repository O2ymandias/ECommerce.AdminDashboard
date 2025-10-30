namespace ECommerce.Core.Common.Options;
public class StripeOptions
{
	public string SecretKey { get; set; }
	public string PublishableKey { get; set; }
	public string WebhookSecret { get; set; }
	public string SuccessRoute { get; set; }
	public string CancelRoute { get; set; }
}
