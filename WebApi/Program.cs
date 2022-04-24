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

app.MapPost("/payments",
    (HttpRequest request,
    [FromHeader(Name = "Authorization")] string? authorization,
    ILoggerFactory loggerFactory) =>
        new PaymentsPost(loggerFactory.CreateLogger("PaymentsPost")).Post(
            request,
            authorization));

app.Logger.LogInformation("The application started");

app.Run();