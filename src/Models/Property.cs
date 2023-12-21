namespace KiboWebhookListener.Models;

public class Property
{
	public string attributeFQN { get; set; }

	public string name { get; set; }

	// ... other properties
	public List<PropertyValue> values { get; set; }
}
