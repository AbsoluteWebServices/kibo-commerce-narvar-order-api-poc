namespace KiboWebhookListener.Models.Narvar;

public class NarvarResponse
{
	public string status { get; set; }
	
	public OrderInfo order_info { get; set; }
	public NarvarMessage[] messages { get; set; }
}
