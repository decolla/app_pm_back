using System.Text.Json;
using System.Text.Json.Serialization;

namespace app_pm_back.Models;
/// <summary>
/// Classe UnixMillisecondsDateTimeConverter.
/// </summary>

public class UnixMillisecondsDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="reader">Parâmetro do método.</param>
    /// <param name="typeToConvert">Parâmetro do método.</param>
    /// <param name="options">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the Unix timestamp in milliseconds and convert it to DateTime
        long milliseconds = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="writer">Parâmetro do método.</param>
    /// <param name="value">Parâmetro do método.</param>
    /// <param name="options">Parâmetro do método.</param>

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Write the DateTime as a Unix timestamp in milliseconds
        long milliseconds = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(milliseconds);
    }
}