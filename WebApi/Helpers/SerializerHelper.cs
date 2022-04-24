using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Helpers;

public static class SerializationExtensions
{
    public static string ToSnakeCase(this string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(paramName: nameof(str));
        }

        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
    public static JsonSerializerOptions Options =>
        new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(new SnakeCaseNamingPolicy()) }
        };
}

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}
