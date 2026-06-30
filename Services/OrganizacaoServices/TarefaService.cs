using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe TarefaService.
/// </summary>
public class TarefaService : BaseCrudService<OperacaoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TarefaService"/>.
    /// </summary>
    public TarefaService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tipoManutencaoId">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<OperacaoModel>> BuscarPorTipoManutencaoAsync(long tipoManutencaoId)
    {
        return await _context.PM_Operacoes
            .Where(t => t.IdTipoManutencao == tipoManutencaoId)
            .OrderBy(t => t.IdOperacao)
            .ToListAsync();
    }
}