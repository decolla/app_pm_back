using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using app_pm_back.Auth;
using app_pm_back.Interfaces;
using app_pm_back.Services;

namespace app_pm_back.Extensions;

/// <summary>
/// Métodos de extensão para configuração de autenticação e autorização via Keycloak.
/// </summary>
public static class KeycloakExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakUrl = configuration["Keycloak:Url"];
        var realm = configuration["Keycloak:Realm"];
        var clientId = configuration["Keycloak:ClientId"];

        if (string.IsNullOrWhiteSpace(keycloakUrl))
            throw new InvalidOperationException("The configuration key Keycloak:Url is missing.");

        if (string.IsNullOrWhiteSpace(realm))
            throw new InvalidOperationException("The configuration key Keycloak:Realm is missing.");

        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("The configuration key Keycloak:ClientId is missing.");

        var authority = $"{keycloakUrl.TrimEnd('/')}/realms/{realm}";

        // valida tokens JWT localmente usando metadata OpenID e chaves públicas JWKS do Keycloak
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = !authority.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authority,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role
                };

                // mapeia roles do client Keycloak
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal?.Identity as ClaimsIdentity;
                        var resourceAccess = context.Principal?.FindFirst("resource_access")?.Value;

                        if (identity is null || string.IsNullOrWhiteSpace(resourceAccess))
                            return Task.CompletedTask;

                        try
                        {
                            using var document = JsonDocument.Parse(resourceAccess);

                            if (!document.RootElement.TryGetProperty(clientId, out var client))
                                return Task.CompletedTask;

                            if (!client.TryGetProperty("roles", out var roles))
                                return Task.CompletedTask;

                            foreach (var role in roles.EnumerateArray())
                            {
                                var roleName = role.GetString();

                                if (!string.IsNullOrWhiteSpace(roleName))
                                    identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            }
                        }
                        catch (JsonException)
                        {
                            // ignora o formato inválido
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        // policies combinam permissões de roles com validação de acesso ao schema
        services.AddAuthorization(options =>
        {
            options.AddPolicy("View", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("view", "edit", "manage");
                policy.AddRequirements(new SchemaAccessRequirement());
            });
            options.AddPolicy("Edit", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("edit", "manage");
                policy.AddRequirements(new SchemaAccessRequirement());
            });
            options.AddPolicy("Manage", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("manage");
                policy.AddRequirements(new SchemaAccessRequirement());
            });

            options.DefaultPolicy = options.GetPolicy("View")!;
        });

        services.AddSingleton<IApplicationSchemaProvider, ApplicationSchemaProvider>();
        services.AddSingleton<IAuthorizationHandler, SchemaAccessAuthorizationHandler>();

        return services;
    }
}
