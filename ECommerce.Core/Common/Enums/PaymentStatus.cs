namespace ECommerce.Core.Common.Enums;
public enum PaymentStatus
{
	Pending,           // Payment not yet initiated (COD before approval or checkout session not started)
	AwaitingPayment,   // Online payment session created, awaiting completion
	PaymentReceived,   // Payment successfully received (online or cash on delivery)
	PaymentFailed      // Online payment failed (declined, canceled, or timed out)
}
