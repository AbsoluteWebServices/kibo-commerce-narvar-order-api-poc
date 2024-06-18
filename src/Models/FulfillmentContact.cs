namespace KiboWebhookListener.Models;

public class FulfillmentContact
{
	
	// We will error out when no email is provided, this is normal
	public string email { get; set; }
	public string firstName { get; set; }
	public string lastNameOrSurname { get; set; }
	public PhoneNumbers phoneNumbers { get; set; }
	public Address address { get; set; }
	

}
