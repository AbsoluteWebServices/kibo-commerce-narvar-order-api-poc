namespace KiboWebhookListener.Models;

public class Card
{
	public string paymentServiceCardId { get; set; }
	public bool isUsedRecurring { get; set; }
	public string nameOnCard { get; set; }
	public bool isCardInfoSaved { get; set; }
	public bool isTokenized { get; set; }
	public string paymentOrCardType { get; set; }
	public string cardNumberPartOrMask { get; set; }
	public int expireMonth { get; set; }
	public int expireYear { get; set; }
}
