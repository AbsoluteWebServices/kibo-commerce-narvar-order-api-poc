namespace KiboWebhookListener.Models;

public class FulfillmentInfo
{
	public string shippingMethodName { get; set; }
	public string shippingMethodCode { get; set; }
	public FulfillmentContact fulfillmentContact { get; set; }

}
