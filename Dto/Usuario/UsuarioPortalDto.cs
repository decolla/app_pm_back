using System.Text.Json.Serialization;

namespace app_pm_back.DTO;

public class PortalAppUserResponseDto
{
    [JsonPropertyName("user")]
    public PortalUserItemDto User { get; set; } = new();

    [JsonPropertyName("accessLevel")]
    public string AccessLevel { get; set; } = string.Empty;
}

public class PortalUserItemDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("cpf")]
    public string? Cpf { get; set; }

    [JsonPropertyName("userRole")]
    public string? UserRole { get; set; }

    [JsonPropertyName("scopes")]
    public List<string> Scopes { get; set; } = new();
}
