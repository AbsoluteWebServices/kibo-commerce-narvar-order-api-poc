using KiboWebhookListener.Helpers;
using KiboWebhookListener.Services;

// Load ENV
EnvHelper.LoadEnv();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var webhookService = new WebhookService();

app.MapPost("/webhooks/kibo", webhookService.ProcessRequest);

app.Run();
