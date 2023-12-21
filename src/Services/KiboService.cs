using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KiboWebhookListener.Exceptions;
using KiboWebhookListener.Models;

namespace KiboWebhookListener.Services;

public class KiboService
{
	private readonly HttpClient _client = new();
	private readonly string _clientId = Environment.GetEnvironmentVariable("KIBO_CLIENT_ID")!;
	private readonly string _clientSecret = Environment.GetEnvironmentVariable("KIBO_CLIENT_SECRET")!;

	private readonly string _sandboxPrefix =
		Environment.GetEnvironmentVariable("KIBO_ENVIRONMENT")!.Equals("Production") ? "" : ".sandbox";

	private readonly string _tenantId = Environment.GetEnvironmentVariable("KIBO_TENANT_ID")!;

	// TODO: pull from environment variables

	public async Task AuthenticateAsync()
	{
		var tokenEndpoint = $"https://{_tenantId}{_sandboxPrefix}.mozu.com/api/platform/applications/authtickets/oauth";

		var requestBody = new
		{
			client_id = _clientId,
			client_secret = _clientSecret,
			grant_type = "client_credentials"
		};
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

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", responseObject["access_token"].ToString());
	}

	public async Task<Order> GetOrderAsync(string orderId)
	{
		await AuthenticateAsync();

		var url = $"https://{_tenantId}{_sandboxPrefix}.mozu.com/api/commerce/orders/{orderId}";
		var response = await _client.GetAsync(url);

		if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error: " + response.StatusCode);

		var responseBody = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Order>(responseBody) ?? throw new KiboException(response);
	}

	public async Task<KiboWebhookData?> DeserializeWebhookData(Stream json)
	{
		return await JsonSerializer.DeserializeAsync<KiboWebhookData>(json);
	}

	public static string TransformResponse(Order response)
	{
		var transformed = new
		{
			order_info = new
			{
				order_number = response.orderNumber,
				order_date = response.submittedDate,
				currency_code = response.currencyCode,
				attributes = new
				{
					order_id = response.id,
					logged_in = response.data.loggedInUser,
					points_to_be_earned = response.data.pointsToBeEarned,
				},
				selected_ship_method = response.fulfillmentInfo.shippingMethodName,
				order_items = response.items.Select(item => new
				{
					item_id = item.id,
					sku = item.product.productCode,
					name = item.product.name,
					description = item.product.description,
					quantity = item.quantity,
					// unit_price = item.UnitPrice,
					// categories = item.Categories,
					// item_image = item.ItemImage,
					// item_url = item.ItemUrl,
					// is_final_sale = item.IsFinalSale,
					// fulfillment_status = item.FulfillmentStatus,
					// is_gift = item.IsGift,
					// attributes = item.Attributes,
					// color = item.Color,
					// events = item.Events.Select(e => new {
					//     eventx = e.EventName,
					//     quantity = e.Quantity,
					//     sequence = e.Sequence
					// })
				}),

				billing = new {
				    billed_to = new {
				        first_name = response.billingInfo.billingContact.firstName,
				        last_name = response.billingInfo.billingContact.lastNameOrSurname,
				        phone = response.billingInfo.billingContact.phoneNumbers.mobile,
				        email = response.billingInfo.billingContact.email,
				        address = new
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
				// TODO: Should this be fulfillmentInfo.fulfillmentContact instead?
				customer = new {
				    first_name = response.billingInfo.billingContact.firstName,
				    last_name = response.billingInfo.billingContact.lastNameOrSurname,
				    phone = response.billingInfo.billingContact.phoneNumbers.mobile,
				    email = response.billingInfo.billingContact.email,
				    address = new
				    {
					    street_1 = response.billingInfo.billingContact.address.address1,
					    street_2 = response.billingInfo.billingContact.address.address2,
					    city = response.billingInfo.billingContact.address.cityOrTown,
					    state = response.billingInfo.billingContact.address.stateOrProvince,
					    zip = response.billingInfo.billingContact.address.postalOrZipCode,
					    country = response.billingInfo.billingContact.address.countryCode
				    },
				    customer_id = response.billingInfo.billingContact.id
				},
			}
		};

		return JsonSerializer.Serialize(transformed);
	}
}
