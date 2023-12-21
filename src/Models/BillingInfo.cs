namespace KiboWebhookListener.Models;

public class BillingInfo
{
	public string paymentType { get; set; }
	public BillingContact billingContact { get; set; }
	public Card card { get; set; }
	public bool isSameBillingShippingAddress { get; set; }

}
