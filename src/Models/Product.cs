namespace KiboWebhookListener.Models;

public class Product
{
	public string upc { get; set; }

	// ... other properties
	public List<string> fulfillmentTypesSupported { get; set; }
	public string imageUrl { get; set; }
	public List<Property> properties { get; set; }
	public List<Category> categories { get; set; }

	public Price price { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string productCode { get; set; }
	// ... other properties
}
