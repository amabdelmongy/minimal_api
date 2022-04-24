# Minimal APIs with ASP.NET Core

Minimal APIs are architected to create HTTP APIs with minimal dependencies.

It has become much easier to build lightweight HTTP applications without MVC, but there are features commonly needed that you would otherwise have to build yourself.

There are no files needed like `startup` and even if controllers folder is not needed.

Only one file as entry to all project `Program.cs` then custimze your project as you need.

We could use MapHealthChecks as MVC
`app.MapHealthChecks("/health");`

Map post to new class. pass `HttpRequest` and `FromHeader` we could read value like `Authorization`.

```
app.MapPost("/payments",
    (HttpRequest request,
    [FromHeader(Name = "Authorization")] string? authorization,
    ILoggerFactory loggerFactory) =>
        new PaymentsPost(loggerFactory.CreateLogger("PaymentsPost")).Post(
            request,
            authorization));
```

Insdate of contrller we could rename it to service or we could use controller.

It returns `IResult` to include `statusCode`. like Unauthorized `401` or Created `201`.

```
public async Task<IResult> Post(
        HttpRequest request,
        string? authorization)
```

Serilization wieh Snake case. We want to return serilizated as snake case.
```
{
    "payment_id": "981026c48899494fb31260bf6ebd45fe",
    "create_date": "2022-04-24T18:54:16.3004302+02:00",
    "status": "created"
}
```

In MVC you can customize the JSON via the AddJsonOptions extension:

```
services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy());
```

With Minmal Api It doesn't work like that. work around we could path Options to the returned `Result`
```
    public static JsonSerializerOptions Options =>
        new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(new SnakeCaseNamingPolicy()) }
        };
```
In this case we are using `SnakeCaseNamingPolicy`. `DefaultIgnoreCondition` ignore null for return values. Converters to `JsonStringEnumConverter`.

`SnakeCaseNamingPolicy` depend on hande coded extination method to convert string to snake case.

```
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}
```
`ToSnakeCase` to convert any string to snake case

```
public static string ToSnakeCase(this string str)
{
    if (str == null)
    {
        throw new ArgumentNullException(paramName: nameof(str));
    }

    return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
}
```

Then we could apply this options when return result.

```
Results.Json(
            statusCode: StatusCodes.Status201Created,
            data: GetAcceptedResponse(),
            options: SerializationExtensions.Options);
```

for future work we could implement our `IResult` that contains `Options` included and returned from our classes.

for logging we could use `Serilog` and configure it at `Program.cs`

```
// remove default logging providers
builder.Logging.ClearProviders();
// Serilog configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
// Register Serilog
builder.Logging.AddSerilog(logger);
```

Minimal APIs Pros
* Code is very light and we could use what we write excatly without for a lot of unnessery code.
* One entry point for all Project `Program.cs`

Minimal APIs cons
* Minimal APIs project fits very samll project for now like scripts
* There are not a lot of exsitantions for minimal APIs for now. When wanting any new cusimtization like serilazation `snake_case` we are going to write it from sckratche with a lot of work around.