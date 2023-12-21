namespace KiboWebhookListener.Models;

public class Payment
{
	public string Id { get; set; }

	// ... other properties
	public GroupId GroupId { get; set; }

	public List<Interaction> Interactions { get; set; }
	// ... other properties
}
