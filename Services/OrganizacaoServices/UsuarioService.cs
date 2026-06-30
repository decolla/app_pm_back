using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe UsuarioService.
/// </summary>
public class UsuarioService : BaseCrudService<UsuarioModel>
{
    private readonly ILogger<UsuarioService> _logger;
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="UsuarioService"/>.
    /// </summary>
    public UsuarioService(AppDbContext context, ILogger<UsuarioService> logger) : base(context)
    {
        _logger = logger;
    }

    /// <summary>
    /// Atribui a reponsabilidade de uma ordem para um usuário
    /// </summary>
    public async Task<bool> AtribuirPermissao(string? nomeUser, long? ordem)
    {
        try
        {
            var usuario = await _context.Set<UsuarioModel>().FirstOrDefaultAsync(u => u.Nome == nomeUser);
            if (usuario != null)
            {
                var ordemModel = await _context.Set<OrdemModel>().FirstOrDefaultAsync(o => o.IdOrdem == ordem);
                if (ordemModel != null)
                {
                    usuario.Ordens.Add(ordemModel);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
                return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar responsabilidade da ordem {ordem} ao usuário {nomeUser}", ordem,nomeUser);
            throw;
        }


    }
}