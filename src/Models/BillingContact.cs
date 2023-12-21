namespace KiboWebhookListener.Models;

public class BillingContact
{
	public int id { get; set; }

	public string email { get; set; }
	public string firstName { get; set; }
	public string lastNameOrSurname { get; set; }
	public PhoneNumbers phoneNumbers { get; set; }
	public Address address { get; set; }

}
