using Microsoft.AspNetCore.Authorization;

namespace app_pm_back.Auth;

/// <summary>
/// Requisito de autorização que exige que o usuário pertença ao schema configurado da aplicação.
/// </summary>
public class SchemaAccessRequirement : IAuthorizationRequirement
{
}
