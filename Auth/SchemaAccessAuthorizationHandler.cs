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
        var schema = schemaProvider.Schema;
        var organizationClaims = context.User.FindAll("organization").Select(c => c.Value).ToList();

        if (organizationClaims.Count == 0)
            return Task.CompletedTask;

        // formato 1: claim multivalorado ["lri", "comgas", ...] (o seu token atual)
        if (organizationClaims.Any(v => string.Equals(v, schema, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // formato 2: objeto JSON {"comgas": {...}}
        foreach (var value in organizationClaims)
        {
            try
            {
                using var document = JsonDocument.Parse(value);
                if (document.RootElement.ValueKind == JsonValueKind.Object &&
                    document.RootElement.TryGetProperty(schema, out _))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            catch (JsonException)
            {
                // valor que não é JSON, ignora e segue para o próximo
            }
        }

        return Task.CompletedTask;
    }
}