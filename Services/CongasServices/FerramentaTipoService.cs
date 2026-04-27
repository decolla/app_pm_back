using app_pm_back.Data;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

/// <summary>
/// Servico de ferramentas por tipo de manutencao.
/// </summary>
public class FerramentaTipoService : BaseCrudService<FerramentasTipoModel>
{
    /// <summary>
    /// Inicializa o servico.
    /// </summary>
    public FerramentaTipoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Lista ferramentas por tipo de manutencao.
    /// </summary>
    public async Task<List<FerramentasTipoModel>> BuscarPorTipoManutencaoAsync(int tipoManutencaoId)
    {
        return await _context.Set<FerramentasTipoModel>()
            .Where(f => f.TipoManutencaoId == tipoManutencaoId)
            .OrderBy(f => f.IdFerramenta)
            .ToListAsync();
    }
}

