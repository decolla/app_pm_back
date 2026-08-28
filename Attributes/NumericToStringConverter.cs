using System.Text.Json;
using System.Text.Json.Serialization;

namespace app_pm_back.Attributes;

/// <summary>
/// Converte valores numéricos, booleanos ou strings JSON em string C#.
/// Útil para tolerar payloads onde campos string são enviados como números pelo cliente.
/// </summary>
public class NumericToStringConverter : JsonConverter<string>
{
    /// <inheritdoc />
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out long intVal))
                {
                    return intVal.ToString();
                }
                if (reader.TryGetDouble(out double doubleVal))
                {
                    return doubleVal.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                return reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture);
            case JsonTokenType.True:
                return "true";
            case JsonTokenType.False:
                return "false";
            case JsonTokenType.Null:
                return null;
            default:
                using (var document = JsonDocument.ParseValue(ref reader))
                {
                    return document.RootElement.GetRawText();
                }
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
