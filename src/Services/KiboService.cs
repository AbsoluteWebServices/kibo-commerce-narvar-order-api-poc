using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KiboWebhookListener.Exceptions;
using KiboWebhookListener.Models;
using KiboWebhookListener.Models.Narvar;

namespace KiboWebhookListener.Services;

public class KiboService
{
	private readonly HttpClient _client = new();
	private readonly string _clientId = Environment.GetEnvironmentVariable("KIBO_CLIENT_ID")!;
	private readonly string _clientSecret = Environment.GetEnvironmentVariable("KIBO_CLIENT_SECRET")!;
	private readonly string _orderPrefix = "UAT_";
	private readonly bool _sandbox = Environment.GetEnvironmentVariable("KIBO_ENVIRONMENT") != "production";

	private readonly string _sandboxPrefix =
		Environment.GetEnvironmentVariable("KIBO_ENVIRONMENT")!.Equals("production") ? "" : ".sandbox";

	private readonly string _tenantId = Environment.GetEnvironmentVariable("KIBO_TENANT_ID")!;

    // TODO: pull from environment variables
    public async Task<KiboWebhookData?> DeserializeWebhookData(Stream json)
    {
        return await JsonSerializer.DeserializeAsync<KiboWebhookData>(json);
    }

    public async Task<KiboOrder> GetOrderAsync(string orderId)
	{
		await AuthenticateAsync();

		var url = $"https://{_tenantId}{_sandboxPrefix}.mozu.com/api/commerce/orders/{orderId}";
		// try to get the order 3 times with a exponential delay
		var attempts = 0;
		while (attempts < 3)
		{
			try
			{
				var response = await _client.GetAsync(url);

				if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error: " + response.StatusCode);

				var responseBody = await response.Content.ReadAsStreamAsync();
				KiboOrder order = await JsonSerializer.DeserializeAsync<KiboOrder>(responseBody) ??
				                         throw new KiboException(response);
				order.orderNumber = _orderPrefix + order.orderNumber;
				return order;
			}
			catch (HttpRequestException e)
			{
				attempts++;
				// log
				Console.WriteLine(e.Message);
				await Task.Delay((int) Math.Pow(2, attempts) * 1000);
			}
		}

		throw new Exception("Could not find Kibo order: " + orderId);
	}

    public async Task<List<KiboShipment>> GetOrderShipmentsAsync(string orderId)
    {
        await AuthenticateAsync();
        
        var url = $"https://{_tenantId}{_sandboxPrefix}.mozu.com/api/commerce/shipments?filter=orderId=={orderId};shipmentStatus!=REASSIGNED;shipmentStatus!=CANCELED;shipmentType!=Transfer;fulfillmentStatus==Fulfilled";
        var response = await _client.GetAsync(url);

        if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error: " + response.StatusCode);

        var responseBody = await response.Content.ReadAsStreamAsync();
        var kiboResponse = await JsonSerializer.DeserializeAsync<ShipmentData>(responseBody) ??
                           throw new KiboException(response);
        // embedded is potentially blank
        return kiboResponse._embedded?.shipments ?? new List<KiboShipment>();
    }

    private async Task AuthenticateAsync()
    {
        var tokenEndpoint = $"https://{_tenantId}{_sandboxPrefix}.mozu.com/api/platform/applications/authtickets/oauth";

        var requestBody = new
        {
            client_id = _clientId,
            client_secret = _clientSecret,
            grant_type = "client_credentials"
        };
        _client.DefaultRequestHeaders.Authorization = null;
        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json")
        };
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
        var token = responseObject?["access_token"].ToString();
		if (token is null)
		{
			throw new Exception("Could not authenticate with Kibo");
		}
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public NarvarRequest TransformOrderResponse(KiboOrder response, List<KiboShipment>? kiboShipments)
	{
		var shipments = (kiboShipments ?? new List<KiboShipment>()).Select((shipment, index) => new NarvarShipment()
		{
			// ship_source = "Kibo",
			items_info = shipment.items.Select(item => new ItemsInfo()
			{
				item_id = item.originalOrderItemId,
				sku = item.productCode,
				quantity = item.quantity
			}).ToList(),
			ship_method = shipment.shippingMethodName,
			// Find first tracking number that is not empty
			
			// Sandbox will always be UPS
			// New comment
			carrier = shipment.packages.SelectMany(p => p.trackingNumbers).First(t => !string.IsNullOrEmpty(t)).StartsWith("1Z") && !_sandbox ? "USPS" : "UPS",
			shipped_to = new ShippedTo()
			{
				first_name = shipment.customer.customerContact.firstName,
				last_name = shipment.customer.customerContact.lastNameOrSurname,
				phone = shipment.customer.customerContact.phoneNumbers.mobile,
				email = shipment.email,
				address = new Models.Narvar.Address()
				{
					street_1 = shipment.destination.destinationContact.address.address1,
					street_2 = shipment.destination.destinationContact.address.address2,
					city = shipment.destination.destinationContact.address.cityOrTown,
					state = shipment.destination.destinationContact.address.stateOrProvince,
					zip = shipment.destination.destinationContact.address.postalOrZipCode,
					country = shipment.destination.destinationContact.address.countryCode
				}
			},
			// TODO: map to kibo order discount?
			ship_discount = 0,
			// ship_total = shipment.total,
			ship_tax = shipment.shippingTaxTotal,
			ship_date = shipment.fulfillmentDate,
			tracking_number = _sandbox ? response.orderNumber + "_" + index : shipment.packages.SelectMany(p => p.trackingNumbers).First(t => !string.IsNullOrEmpty(t))
		}).ToList();
		
		NarvarRequest transformed = new NarvarRequest()
		{
			order_info = new OrderInfo()
			{
				order_number = response.orderNumber,
				order_date = response.submittedDate,
				currency_code = response.currencyCode,
				attributes = new OrderAttributes()
				{
					order_id = response.id,
					logged_in = response.data.loggedInUser ? "true" : "false",
					points_to_be_earned = response.data.pointsToBeEarned.ToString(),
				},
				// selected_ship_method = response.fulfillmentInfo.shippingMethodName,
				order_items = new List<OrderItem>(response.items.Select(item => new OrderItem
				{
					item_id = item.id,
					sku = item.product.productCode,
					name = item.product.name,
					description = item.product.description,
					quantity = item.quantity,
					unit_price = (double)item.product.price.price,
					item_image = item.product.imageUrl.StartsWith("//") ? "https:" + item.product.imageUrl : item.product.imageUrl,
					fulfillment_status = (kiboShipments ?? new List<KiboShipment>())
						.Any(s => s.items.Any(i => i.productCode == item.product.productCode)) 
						? "SHIPPED" : "NOT_SHIPPED"
				})),
				billing = new Billing()
				{
					billed_to = new ContactWithAddress()
					{
						first_name = response.billingInfo.billingContact.firstName,
						last_name = response.billingInfo.billingContact.lastNameOrSurname,
						phone = response.billingInfo.billingContact.phoneNumbers.mobile,
						email = response.billingInfo.billingContact.email,
						address = new Models.Narvar.Address()
						{
							street_1 = response.billingInfo.billingContact.address.address1,
							street_2 = response.billingInfo.billingContact.address.address2,
							city = response.billingInfo.billingContact.address.cityOrTown,
							state = response.billingInfo.billingContact.address.stateOrProvince,
							zip = response.billingInfo.billingContact.address.postalOrZipCode,
							country = response.billingInfo.billingContact.address.countryCode
						}
					}
				},
				shipments = shipments,
				customer = new ContactWithAddressAndId()
				{
					first_name = response.billingInfo.billingContact.firstName,
					last_name = response.billingInfo.billingContact.lastNameOrSurname,
					phone = response.billingInfo.billingContact.phoneNumbers.mobile,
					email = response.billingInfo.billingContact.email,
					address = new Models.Narvar.Address()
					{
						street_1 = response.billingInfo.billingContact.address.address1,
						street_2 = response.billingInfo.billingContact.address.address2,
						city = response.billingInfo.billingContact.address.cityOrTown,
						state = response.billingInfo.billingContact.address.stateOrProvince,
						zip = response.billingInfo.billingContact.address.postalOrZipCode,
						country = response.billingInfo.billingContact.address.countryCode
					},
					customer_id = response.userId
				}
			}
		};

		return transformed;
	}
}
