namespace KiboWebhookListener.Models;

public class Note
{
	public string Id { get; set; }
	public string Text { get; set; }

	public AuditInfo AuditInfo { get; set; }
	// ... other properties
}
