using KiboWebhookListener.Exceptions;
using KiboWebhookListener.Models;

namespace KiboWebhookListener.Services;

public class WebhookService
{
	private readonly KiboService _kiboService = new();
	private readonly NarvarService _narvarService = new();

	public async Task<IResult> ProcessRequest(HttpRequest request)
	{
		var jsonData = await _kiboService.DeserializeWebhookData(request.Body);
		if (!IsValidJson(jsonData))
			return Results.BadRequest("JSON deserialization resulted in null.");

		if (jsonData!.body.topic.StartsWith("order"))
			return await ProcessOrder(jsonData);

		return Results.BadRequest("No route found for this webhook topic.");

	}

	private bool IsValidJson(KiboWebhookData? jsonData)
	{
		return jsonData?.body != null;
	}

	private async Task<IResult> ProcessOrder(KiboWebhookData jsonData)
	{
		try
		{
			var response = await _kiboService.GetOrderAsync(jsonData.body.entityId);
			var transformedResponse = KiboService.TransformResponse(response);
			var narvarResponse = await _narvarService.PostOrderAsync(transformedResponse);
			return Results.Json(narvarResponse);
		}
		catch (KiboException e)
		{
			LogException(e);
			return Results.Json(e);
		}
		catch (NarvarException e)
		{
			LogException(e);
			return Results.Json(e.JsonParameter);
		}
	}

	private void LogException(Exception e)
	{
		Console.WriteLine($"\nException Caught! Message :{e.Message}");
	}
}
