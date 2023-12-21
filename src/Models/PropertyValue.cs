namespace KiboWebhookListener.Models;

public class PropertyValue
{
	public string stringValue { get; set; }

	// This can be string, boolean, w/e
	public object value { get; set; }
	// ... other properties
}
