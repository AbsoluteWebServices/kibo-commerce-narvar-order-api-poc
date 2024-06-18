namespace KiboWebhookListener.Models;

public class TenantConfig
{
	public string? clientId { get; set; }
	public string? clientSecret { get; set; }
	public string environment { get; set; }
	public string? orderPrefix { get; set; }
}
