using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using app_pm_back.Interfaces;

namespace app_pm_back.Auth;

/// <summary>
/// Validador que verifica se o usuário autenticado possui acesso ao schema configurado no claim 'organization' do JWT.
/// </summary>
public class SchemaAccessAuthorizationHandler(IApplicationSchemaProvider schemaProvider)
    : AuthorizationHandler<SchemaAccessRequirement>
{
    // valida se o usuário autenticado tem acesso ao schema configurado para esta aplicação
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SchemaAccessRequirement requirement)
    {
        // o Keycloak envia os schemas/organizações permitidas no claim organization
        var organizationClaim = context.User.FindFirst("organization")?.Value;

        if (string.IsNullOrWhiteSpace(organizationClaim))
            return Task.CompletedTask;

        try
        {
            using var document = JsonDocument.Parse(organizationClaim);

            // acesso é liberado apenas se o schema configurado existir dentro do claim organization
            if (document.RootElement.ValueKind == JsonValueKind.Object &&
                document.RootElement.TryGetProperty(schemaProvider.Schema, out _))
            {
                context.Succeed(requirement);
            }
        }
        catch (JsonException)
        {
            //se o claim for invaĺido, o usuário não tem acesso
        }

        return Task.CompletedTask;
    }
}