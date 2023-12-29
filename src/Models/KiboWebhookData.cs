namespace KiboWebhookListener.Models;

public class KiboWebhookData
{
	public string eventId { get; set; }
	public string topic { get; set; }
	public string entityId { get; set; }
	public string timestamp { get; set; }
	public string correlationId { get; set; }
	public bool isTest { get; set; }
}
