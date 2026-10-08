using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using app_pm_back.Interfaces;
using app_pm_back.Config;
using Microsoft.IdentityModel.Tokens;

namespace app_pm_back.Services;

public class TokenService : ITokenService
{
    private string? _cachedAdminToken;
    private DateTime _tokenExpirationTime = DateTime.MinValue;
    private readonly KeycloakConfig _keycloak; 
    private readonly HttpClient _httpClient;

    public TokenService(HttpClient httpClient, IOptions<KeycloakConfig> options)
    {
        _httpClient = httpClient;
        _keycloak = options.Value;
    }

    /// <summary>
    /// Mapeia as respostas de erro do Keycloak.
    /// </summary>
    /// <param name="ErrorType"></param>
    /// <param name="ErrorDescription"></param>
    public readonly record struct KeycloakErrorResponse(
        [property: JsonPropertyName("error")] KeycloakErrorResponse.Error ErrorType, 
        [property: JsonPropertyName("error_description")] string? ErrorDescription)
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Error
        {
            [EnumMember(Value = "invalid_request")] INVALID_REQUEST,
            [EnumMember(Value = "invalid_client")] INVALID_CLIENT,
            [EnumMember(Value = "invalid_grant")] INVALID_GRANT,
            [EnumMember(Value = "unauthorized_client")] UNAUTHORIZED_CLIENT,
            [EnumMember(Value = "unsupported_grant_type")] UNSUPPORTED_GRANT_TYPE,
            [EnumMember(Value = "invalid_scope")] INVALID_SCOPE
        }
    }
    
    /// <summary>
    /// Gera um token JWT para autenticação com o APM
    /// </summary>
    /// <returns>Token gerado</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GenerateToken()
    {
        string _secret;
        _secret = Environment.GetEnvironmentVariable("TWINS_KEY") ?? 
                  throw new InvalidOperationException("TWINS_KEY não foi configurada");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: "http://0.0.0.0:5000",
            audience: "http://0.0.0.0:5000",
            expires: DateTime.UtcNow.AddMinutes(5),
            claims: new[]
            {
                new Claim("service", "UnitAnalyticsBack")
            },
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Mapeia respostas de token do Keycloak.
    /// </summary>
    public readonly record struct TokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("id_token")] string? IdToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("refresh_expires_in")] int? RefreshExpiresIn,
        [property: JsonPropertyName("token_type")] string TokenType, 
        [property: JsonPropertyName("scope")] string Scope);

    
    /// <summary>
    /// Gera um token de acesso admin para o Keycloak.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<string> GetAdminAccessTokenAsync()
    {
        // se o token ainda é válido retorna do cache na memória
        if (_cachedAdminToken is not null && _tokenExpirationTime > DateTime.UtcNow.AddSeconds(30))
            return _cachedAdminToken;

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", _keycloak.ClientId },
            { "client_secret", _keycloak.ClientSecret },
            { "grant_type", "client_credentials" },
            { "scope", $"portal-backend-aud {_keycloak.ClientId}-aud" }
        });

        // URL do Keycloak para obter o token de acesso admin
        var response = await _httpClient.PostAsync($"{_keycloak.Url}/realms/{_keycloak.Realm}/protocol/openid-connect/token", content);
        
        if (response.StatusCode is HttpStatusCode.OK)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            
            _cachedAdminToken = tokenResponse.AccessToken;
            _tokenExpirationTime = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            
            return _cachedAdminToken;
        }

        // tratamento de erros 
        KeycloakErrorResponse? errorContent;
        try
        {
            errorContent = await response.Content.ReadFromJsonAsync<KeycloakErrorResponse>();
        }
        catch
        {
            errorContent = null;
        }

        var rawContent = errorContent?.ToString() ?? await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Erro ao obter token M2M no Keycloak (Status: {response.StatusCode}): {rawContent}", null, response.StatusCode);
    }
}