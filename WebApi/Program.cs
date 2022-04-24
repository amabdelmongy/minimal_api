using Microsoft.AspNetCore.Mvc;
using Serilog;

// Help to follow up with minimal api
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

// remove default logging providers
builder.Logging.ClearProviders();
// Serilog configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
// Register Serilog
builder.Logging.AddSerilog(logger);

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapPost("/{paymentMethodIdentifier}/payments",
    (HttpRequest request,
    string paymentMethodIdentifier,
    [FromHeader(Name = "Authorization")] string? authorization,
    [FromHeader(Name = "Cko-Merchant-Data")] string? ckoMerchantData,
    [FromHeader(Name = "Cko-Payment-Signature")] string? ckoPaymentSignature,
    ILoggerFactory loggerFactory) =>
        new PaymentsPost(loggerFactory.CreateLogger("PaymentsPost")).Post(
            request,
            paymentMethodIdentifier,
            authorization,
            ckoMerchantData,
            ckoPaymentSignature));

app.Logger.LogInformation("The application started");

app.Run();