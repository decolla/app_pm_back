using app_pm_back.Data;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

/// <summary>
/// Servico de itens de renovacao por tipo de manutencao.
/// </summary>
public class ItemRenovacaoTipoService : BaseCrudService<ItemRenovacaoTipoModel>
{
    /// <summary>
    /// Inicializa o servico.
    /// </summary>
    public ItemRenovacaoTipoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Lista itens por tipo de manutencao.
    /// </summary>
    public async Task<List<ItemRenovacaoTipoModel>> BuscarPorTipoManutencaoAsync(int tipoManutencaoId)
    {
        return await _context.Set<ItemRenovacaoTipoModel>()
            .Where(i => i.TipoManutencaoId == tipoManutencaoId)
            .OrderBy(i => i.IdItemRenovacaoTipo)
            .ToListAsync();
    }
}

