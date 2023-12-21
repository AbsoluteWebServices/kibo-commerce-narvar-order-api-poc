using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KiboWebhookListener.Exceptions;
using KiboWebhookListener.Models.Narvar;

namespace KiboWebhookListener.Services;

public class NarvarService
{
	private const string BaseEndpoint = "https://ws.narvar.com/api/v1";
	private readonly HttpClient _client = new();
	private readonly string _password = Environment.GetEnvironmentVariable("NARVAR_AUTH_TOKEN")!;
	private readonly string _username = Environment.GetEnvironmentVariable("NARVAR_ACCOUNT_ID")!;

	public NarvarService()
	{
		if (_password == null || _username == null)
			throw new NarvarException("credentials.invalid", "Narvar environment variables not set in .env");

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));
	}

	public async Task<NarvarResponse?> PostOrderAsync(string payload)
	{
		var response = await _client.PostAsync($"{BaseEndpoint}/orders",
			new StringContent(payload, Encoding.UTF8, "application/json"));

		if (!response.IsSuccessStatusCode)
			// Read json response body
			throw new NarvarException(response);

		var responseBody = await response.Content.ReadAsStreamAsync();
		// TODO: Return Narvar Order
		return await JsonSerializer.DeserializeAsync<NarvarResponse>(responseBody);
	}
}
