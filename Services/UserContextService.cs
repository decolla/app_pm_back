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
        // dps substituir IdEscopo pelo nome exato gerado
        var claimsEscopo = user.FindAll("IdEscopo"); 
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