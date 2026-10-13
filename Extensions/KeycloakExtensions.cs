using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using app_pm_back.Auth;
using app_pm_back.Data;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.EntityFrameworkCore;

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
        
        var allowInsecure = configuration.GetValue<bool>("Keycloak:AllowInsecureCertificate");

        // valida tokens JWT localmente usando metadata OpenID e chaves públicas JWKS do Keycloak
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = !authority.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
                options.MapInboundClaims = false;
                
                if (allowInsecure)
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }


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
                    OnTokenValidated = async context =>
                    {var identity = context.Principal?.Identity as ClaimsIdentity;
                        var resourceAccess = context.Principal?.FindFirst("resource_access")?.Value;
                        
                        if (identity != null && !string.IsNullOrWhiteSpace(resourceAccess))
                        {
                            try
                            {
                                using var document = JsonDocument.Parse(resourceAccess);
                                if (document.RootElement.TryGetProperty(clientId, out var client) && 
                                    client.TryGetProperty("roles", out var roles))
                                {
                                    foreach (var role in roles.EnumerateArray())
                                    {
                                        var roleName = role.GetString();
                                        if (!string.IsNullOrWhiteSpace(roleName))
                                            identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                                    }
                                }
                            }
                            catch (JsonException) { }
                        }

                        // cadastro automático de usuários no banco de dados se não existirem
                        if (identity != null)
                        {
                            var portalId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                        ?? context.Principal?.FindFirst("sub")?.Value;
                            
                            if (!string.IsNullOrEmpty(portalId))
                            {
                                // obtém o DbContext apenas para este request
                                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                                bool usuarioExiste = await dbContext.Set<UsuarioModel>().AnyAsync(u => u.PortalId == portalId);

                                if (!usuarioExiste)
                                {
                                    var name = context.Principal?.FindFirst("name")?.Value 
                                            ?? context.Principal?.FindFirst("preferred_username")?.Value;

                                    var novoUsuario = new UsuarioModel
                                    {
                                        PortalId = portalId,
                                        Nome = name ?? "Utilizador Auto-Cadastrado",
                                        NivelPermissao = NivelPermissao.Usuario, // acesso mínimo até a sincronização diária atualizar
                                    };

                                    dbContext.Set<UsuarioModel>().Add(novoUsuario);
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                        }
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
