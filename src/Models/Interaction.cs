namespace KiboWebhookListener.Models;

public class Interaction
{
	public string Id { get; set; }

	// ... other properties
	public List<GatewayResponseData> GatewayResponseData { get; set; }

	public AuditInfo AuditInfo { get; set; }
	// ... other properties
}
