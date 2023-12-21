namespace KiboWebhookListener.Models;

public class Address
{
	public string address1 { get; set; }
	public string address2 { get; set; }
	public string cityOrTown { get; set; }
	public string stateOrProvince { get; set; }
	public string postalOrZipCode { get; set; }
	public string countryCode { get; set; }
	public string addressType { get; set; }
	public bool isValidated { get; set; }
}
