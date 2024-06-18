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
	private string? _orderPrefix;
	private string? _clientId;
	private string? _clientSecret;
	private bool _sandbox;
	private static readonly TokenManager _tokenManager = new TokenManager();


	// private readonly string _tenantId = Environment.GetEnvironmentVariable("KIBO_TENANT_ID")!;
	private string? _baseUrl;
	
	
	// Constructor to process tenantConfig
	public KiboService(string tenantIdRaw)
	{
		var tenantId = "t" + tenantIdRaw;
		// TENANT_t15653_CLIENT_ID=cb_narvar_order_api_uat.2.0.0.Release
		// TENANT_t15653_CLIENT_SECRET=a2171a18567840b79777cb8b09a93559
		// TENANT_t15653_ENVIRONMENT=production
		// TENANT_t15653_ORDER_PREFIX=PROD_
		var rawConfig = $@"{{
			""{tenantId}"": {{
				""clientId"": ""{Environment.GetEnvironmentVariable($"TENANT_{tenantId}_CLIENT_ID")}"",
				""clientSecret"": ""{Environment.GetEnvironmentVariable($"TENANT_{tenantId}_CLIENT_SECRET")}"",
				""environment"": ""{Environment.GetEnvironmentVariable($"TENANT_{tenantId}_ENVIRONMENT")}"",
				""orderPrefix"": ""{Environment.GetEnvironmentVariable($"TENANT_{tenantId}_ORDER_PREFIX")}""
			}}
		}}";
		var tenantConfigs = JsonSerializer.Deserialize<Dictionary<String, TenantConfig>>(rawConfig);
		if (tenantConfigs != null && tenantConfigs.TryGetValue(tenantId, out TenantConfig? config))
		{
			_clientId = config.clientId;
			_clientSecret = config.clientSecret;
			_sandbox = config.environment != "production";
			_orderPrefix = config.orderPrefix;
			
			var sandboxPrefix = GetSandboxPrefix(config.environment);
			_baseUrl = $"https://{tenantId}{sandboxPrefix}.mozu.com";
		}
	}
	private string? GetSandboxPrefix(string environment)
	{
		return environment.Equals("production") ? string.Empty : ".sandbox";
	}
	
    public async Task<KiboWebhookData?> DeserializeWebhookData(Stream json)
    {
        return await JsonSerializer.DeserializeAsync<KiboWebhookData>(json);
    }

    public async Task<KiboOrder> GetOrderAsync(string orderId)
	{
		await AuthenticateAsync();

		var url = $"{_baseUrl}/api/commerce/orders/{orderId}";
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
				if (order.status is "Pending")
				{
					throw new Exception("Order is still pending");
				}
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
        
        var url = $"{_baseUrl}/api/commerce/shipments?filter=orderId=={orderId};shipmentStatus!=REASSIGNED;shipmentStatus!=CANCELED;shipmentType!=Transfer;fulfillmentStatus==Fulfilled";
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
	    
	    // Check if the token is still valid
	    if (_tokenManager.Token != null && _tokenManager.Expiration > DateTime.UtcNow)
	    {
		    _client.DefaultRequestHeaders.Authorization =
			    new AuthenticationHeaderValue("Bearer", _tokenManager.Token);
		    return;
	    }
        var tokenEndpoint = $"{_baseUrl}/api/platform/applications/authtickets/oauth";

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
		var expiresIn = Convert.ToInt32(responseObject?["expires_in"].ToString());
		_tokenManager.Token = token;
		_tokenManager.Expiration = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Subtract 60 seconds to ensure renewal before expiration

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", _tokenManager.Token);
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
			carrier = shipment.packages.SelectMany(p => p.trackingNumbers).First(t => !string.IsNullOrEmpty(t)).StartsWith("1Z") ? "UPS" : "USPS",
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
			tracking_number = _sandbox 
				? response.orderNumber + "_" + index 
				: shipment.packages
					.SelectMany(p => p.trackingNumbers)
					.FirstOrDefault(t => !string.IsNullOrEmpty(t))
					?.Split(',')
					.FirstOrDefault(t => !string.IsNullOrEmpty(t))
		}).ToList();
		
		var billingContact = response.billingInfo?.billingContact ?? new BillingContact
		{
			firstName = response.fulfillmentInfo.fulfillmentContact.firstName,
			lastNameOrSurname = response.fulfillmentInfo.fulfillmentContact.lastNameOrSurname,
			phoneNumbers = new Models.PhoneNumbers()
			{
				mobile = response.fulfillmentInfo.fulfillmentContact.phoneNumbers.mobile
			},
			email = response.fulfillmentInfo.fulfillmentContact.email,
			address = new Models.Address
			{
				address1 = response.fulfillmentInfo.fulfillmentContact.address.address1,
				address2 = response.fulfillmentInfo.fulfillmentContact.address.address2,
				cityOrTown = response.fulfillmentInfo.fulfillmentContact.address.cityOrTown,
				stateOrProvince = response.fulfillmentInfo.fulfillmentContact.address.stateOrProvince,
				postalOrZipCode = response.fulfillmentInfo.fulfillmentContact.address.postalOrZipCode,
				countryCode = response.fulfillmentInfo.fulfillmentContact.address.countryCode
			}
		};

		
		NarvarRequest transformed = new NarvarRequest()
		{
			order_info = new OrderInfo()
			{
				order_number = response.orderNumber,
				// If no submitted date, it isn't ready
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
						first_name = billingContact.firstName,
						last_name = billingContact.lastNameOrSurname,
						phone = billingContact.phoneNumbers.mobile,
						email = billingContact.email,
						address = new Models.Narvar.Address()
						{
							street_1 = billingContact.address.address1,
							street_2 = billingContact.address.address2,
							city = billingContact.address.cityOrTown,
							state = billingContact.address.stateOrProvince,
							zip = billingContact.address.postalOrZipCode,
							country = billingContact.address.countryCode
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

public class TokenManager
{
	public string Token { get; set; }
	public DateTime Expiration { get; set; }
}
