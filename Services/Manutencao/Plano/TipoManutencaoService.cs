using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe TipoManutencaoService.
/// </summary>
public class TipoManutencaoService : BaseCrudService<TipoManutencaoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoManutencaoService"/>.
    /// </summary>
    public TipoManutencaoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<TipoManutencaoModel?> GetCompletoByIdAsync(long id)
    {
        return await _context.PM_TiposManutencao
            .FirstOrDefaultAsync(t => t.IdTipoManutencao == id);
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    /// <param name="atualizado">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<TipoManutencaoModel> UpdateBasicoAsync(TipoManutencaoModel atualizado)
    {
        var existente = await _context.PM_TiposManutencao
            .FirstOrDefaultAsync(t => t.IdTipoManutencao == atualizado.IdTipoManutencao);

        if (existente == null)
        {
            throw new Exception("Registo não encontrado");
        }

        existente.Categoria = atualizado.Categoria;
        existente.Nome = atualizado.Nome;
        existente.TipoLI = atualizado.TipoLI;
        existente.Confiabilidade = atualizado.Confiabilidade;
        existente.Habilitada = atualizado.Habilitada;

        await _context.SaveChangesAsync();
        return existente;
    }
}