namespace app_pm_back.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Obtém um token de acesso válido 
    /// Retorna o token em cache se ainda não estiver expirado
    /// </summary>
    Task<string> GetAdminAccessTokenAsync();
}
