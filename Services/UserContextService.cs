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

    public List<long> GetEscoposDoUsuario()
    {
        // verifica se existe uma requisição HTTP ativa
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user == null || !user.Identity!.IsAuthenticated)
            return new List<long>(); // pode ser o Hangfire ou um endpoint anonimo

        // busca a informação dentro do JWT 
        // aceita IdEscopo ou escopo
        var claimsEscopo = user.FindAll("IdEscopo").Concat(user.FindAll("escopo")); 
        var listaEscopos = new List<long>();
        
        foreach (var claim in claimsEscopo)
        {
            if (long.TryParse(claim.Value, out long escopoId))
            {
                listaEscopos.Add(escopoId);
            }
        }

        return listaEscopos;
    }

    /// <summary>
    /// Captura o Id usuário por meio do contexto HTTP e autenticação
    /// </summary>
    /// <returns></returns>
    public int? GetUsuarioIdAtual()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated) return null;

        // o ID do usuário costuma vir na claim NameIdentifier, sub ou IdUsuario
        var claimId = user.FindFirst(ClaimTypes.NameIdentifier) 
            ?? user.FindFirst("sub") 
            ?? user.FindFirst("IdUsuario");
        
        if (claimId != null && int.TryParse(claimId.Value, out int userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Identifica o usuário por Keycloak
    /// </summary>
    /// <returns></returns>
    public string? GetUsuarioIdentificador()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity!.IsAuthenticated) return null;

        // no Keycloak, o identificador único é o sub e o nome de usuário é "preferred_username"
        return user.FindFirst("preferred_username")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value
            ?? user.FindFirst("IdUsuario")?.Value
            ?? user.Identity?.Name;
    }
}