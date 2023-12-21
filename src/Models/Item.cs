namespace KiboWebhookListener.Models;

public class Item
{
	public string id { get; set; }

	public string sku { get; set; }

	// ... other properties
	public Product product { get; set; }
	public decimal quantity { get; set; }

	public decimal subtotal { get; set; }

	// ... other properties
	public Data data { get; set; }

	public AuditInfo auditInfo { get; set; }
	// ... other properties
}
