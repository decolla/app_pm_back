using app_pm_back.Data;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

/// <summary>
/// Servico de tarefas por tipo de manutencao.
/// </summary>
public class TarefaService : BaseCrudService<TarefaModel>
{
    /// <summary>
    /// Inicializa o servico.
    /// </summary>
    public TarefaService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Lista tarefas por tipo de manutencao.
    /// </summary>
    public async Task<List<TarefaModel>> BuscarPorTipoManutencaoAsync(int tipoManutencaoId)
    {
        return await _context.Set<TarefaModel>()
            .Where(t => t.TipoManutencaoId == tipoManutencaoId)
            .OrderBy(t => t.IdTarefa)
            .ToListAsync();
    }
}

