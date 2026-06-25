using app_pm_back.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace app_pm_back.Services;

public class UserContextService : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? GetEscopoAtual()
    {
        // verifica se existe uma requisição HTTP ativa
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user == null || !user.Identity!.IsAuthenticated)
            return null; // pode ser o Hangfire ou um endpoint anonimo

        // busca a informação dentro do JWT 
        // dps substituir IdEscopo pelo nome exato gerado
        var claimEscopo = user.FindFirst("IdEscopo"); 
        
        if (claimEscopo != null && long.TryParse(claimEscopo.Value, out long escopoId))
        {
            return escopoId;
        }

        return null;
    }

    public int? GetUsuarioIdAtual()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated) return null;

        // o ID do usuário costuma vir na claim NameIdentifier ou em um nome customizado
        var claimId = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("IdUsuario");
        
        if (claimId != null && int.TryParse(claimId.Value, out int userId))
        {
            return userId;
        }

        return null;
    }
}