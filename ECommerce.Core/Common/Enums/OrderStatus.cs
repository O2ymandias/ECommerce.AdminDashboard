namespace ECommerce.Core.Common.Enums;
public enum OrderStatus
{
	Pending,     // Order created, awaiting payment (Card) or approval (Cash)
	Processing,  // Payment received (Card) or confirmed for COD; preparing for shipment
	Shipped,     // Order dispatched
	Delivered,   // Order delivered to customer
	Cancelled    // Order was cancelled at any point
}
