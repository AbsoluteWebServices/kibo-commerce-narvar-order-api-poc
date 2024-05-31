using System.Text.Json.Serialization;
using KiboWebhookListener.Helpers;

namespace KiboWebhookListener.Models;

public class KiboOrder
{

	public string id { get; set; }
	public string userId { get; set; }
	[JsonConverter(typeof(KiboOrderNumberConverter))]
	public string orderNumber { get; set; }
	public string submittedDate { get; set; }
	public string version { get; set; }

	public bool isPartialOrder { get; set; }

	// ... other properties
	public List<Note> notes { get; set; }
	public List<Item> items { get; set; }

	public BillingInfo billingInfo { get; set; }

	public FulfillmentInfo fulfillmentInfo { get; set; }

	public string currencyCode { get; set; }

	public Data data { get; set; }
	// ... other properties
}
