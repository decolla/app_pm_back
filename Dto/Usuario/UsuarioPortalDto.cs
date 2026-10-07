using System.Text.Json.Serialization;

namespace app_pm_back.DTO;

public class PortalPaginatedResponseDto
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public List<PortalUserItemDto> Items { get; set; } = new();
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

    [JsonPropertyName("applicationPermissions")]
    public List<PortalApplicationPermissionDto> ApplicationPermissions { get; set; } = new();
    
    [JsonPropertyName("scopes")]
    public List<string> Scopes { get; set; } = new();
}

public class PortalApplicationPermissionDto
{
    [JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; } = string.Empty;

    [JsonPropertyName("applicationAccessLevel")]
    public string ApplicationAccessLevel { get; set; } = string.Empty;
}