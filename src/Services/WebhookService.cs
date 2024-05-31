using KiboWebhookListener.Exceptions;
using KiboWebhookListener.Models;
using KiboWebhookListener.Models.Narvar;

namespace KiboWebhookListener.Services;

public class WebhookService
{
	private readonly KiboService _kiboService = new();
	private readonly NarvarService _narvarService = new();

	public async Task<IResult> ProcessRequest(HttpRequest request)
	{
		var jsonData = await _kiboService.DeserializeWebhookData(request.Body);
		if (!IsValidJson(jsonData))
		{
			return Results.BadRequest("JSON deserialization resulted in null.");
		}
		try
		{
			switch (jsonData!.topic)
			{
				case "order.opened":
					var order = await CreateNarvarOrderFromKiboWebhook(jsonData);
					return Results.Json(order);
				case "order.fulfilled":
				case "order.updated":
					return await ProcessOrderShipped(jsonData);
				default:
					return Results.BadRequest("No route found for this webhook topic.");
			}
		}
		catch (KiboException e)
		{
			LogException(e);
			return Results.Json(e.Message);
		}
		catch (NarvarException e)
		{
			LogException(e);
			return Results.Json(e.JsonParameter);
		}
	}

    private bool IsValidJson(KiboWebhookData? jsonData)
	{
		return jsonData?.topic != null;
	}

	private async Task<NarvarResponse> CreateNarvarOrderFromKiboWebhook(KiboWebhookData jsonData)
	{
		var response = await _kiboService.GetOrderAsync(jsonData.entityId);
		var transformedResponse = _kiboService.TransformOrderResponse(response, null);
		return await _narvarService.PostOrderAsync(transformedResponse);
	}

    private async Task<IResult> ProcessOrderShipped(KiboWebhookData jsonData)
    {
        try
        {
	        var kiboOrder = await _kiboService.GetOrderAsync(jsonData.entityId);
			// Check if order has been sent to Narvar
			var narvarOrder = await _narvarService.GetOrderAsync(kiboOrder.orderNumber.ToString());
			if (narvarOrder is { status: "FAILURE" })
			{
				narvarOrder = await CreateNarvarOrderFromKiboWebhook(jsonData);
			}

			if (narvarOrder is null)
			{
				return Results.BadRequest("Narvar order not found and could not be created.");
			}
			
            var kiboShipments = await _kiboService.GetOrderShipmentsAsync(jsonData.entityId);
            var newNarvarOrder = _kiboService.TransformOrderResponse(kiboOrder, kiboShipments);
            // compare narvarOrder to newNarvarOrder
            if (narvarOrder.order_info.shipments.Count != newNarvarOrder.order_info.shipments.Count)
			{
				var narvarShipmentResponse = await _narvarService.PostOrderAsync(newNarvarOrder);
				return Results.Json(narvarShipmentResponse);
			}
			else
			{
				// No new shipments
				return Results.Json(new
				{
					message = "No new shipments to process."
				});
			}
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
