# Minimal APIs with ASP.NET Core

Minimal APIs are architected to create HTTP APIs with minimal dependencies.

It has become much easier to build lightweight HTTP applications without MVC, but there are features commonly needed that you would otherwise have to build yourself.

There are no files needed like `startup` and even controllers folder is not needed.

Only one file is entry to all project `Program.cs` then you can custimze your project as you need.

## use MapHealthChecks as MVC
```app.MapHealthChecks("/health");```


## Use Map post to map all urls.
 We could pass `HttpRequest` and read any value from header `FromHeader`.

```
app.MapPost("/payments",
    (HttpRequest request,
    [FromHeader(Name = "Authorization")] string? authorization,
    ILoggerFactory loggerFactory) =>
        new PaymentsPost(loggerFactory.CreateLogger("PaymentsPost")).Post(
            request,
            authorization));
```

## PaymentsPost controller or service
We could create new class to contatin post method.
It returns `IResult` to include `statusCode`. like Unauthorized `401` or Created `201`.
Ther is no need to use controller.

```
public async Task<IResult> Post(
        HttpRequest request,
        string? authorization)
```

## Serialization with Snake case
output we expect to be like

```
{
    "payment_id": "981026c48899494fb31260bf6ebd45fe",
    "create_date": "2022-04-24T18:54:16.3004302+02:00",
    "status": "created"
}
```

### In MVC you can customize the JSON via the AddJsonOptions extension:

```
services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy());
```

### With Minmal Api It doesn't work like that. work around we could path Options to the returned `Result`
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

Minimal API Pros
* The code is very light and we can use what we write with excitement without a lot of standardization code.
* Single entry point for all project "Program.cs"

Minimal Cons of APIs
* A minimal API project fits a very small project right now like scripts
* There are not many extentions for minimal APIs at the moment. When we want any new format like serilazation "snake_case" we will write it from sckratche.