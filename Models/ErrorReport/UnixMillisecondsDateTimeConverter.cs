using System.Text.Json;
using System.Text.Json.Serialization;

namespace app_pm_back.Models;

public class UnixMillisecondsDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the Unix timestamp in milliseconds and convert it to DateTime
        long milliseconds = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write the DateTime as a Unix timestamp in milliseconds
        long milliseconds = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(milliseconds);
    }
}