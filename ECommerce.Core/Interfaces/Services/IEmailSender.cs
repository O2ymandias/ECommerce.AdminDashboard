using ECommerce.Core.Common;

namespace ECommerce.Core.Interfaces.Services;
public interface IEmailSender
{
	Task SendEmailAsync(Email email);
}
