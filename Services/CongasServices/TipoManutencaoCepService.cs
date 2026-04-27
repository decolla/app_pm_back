using app_pm_back.Data;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

/// <summary>
/// Servico de tipo de manutencao CEP.
/// </summary>
public class TipoManutencaoCepService : BaseCrudService<TipoManutencaoCepModel>
{
    /// <summary>
    /// Inicializa o servico.
    /// </summary>
    public TipoManutencaoCepService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca tipos CEP por tipo de ativo.
    /// </summary>
    public async Task<List<TipoManutencaoCepModel>> BuscarPorTipoAtivoAsync(string tipoAtivo)
    {
        var tipoAtivoNormalizado = tipoAtivo.Trim().ToLower();

        return await _context.Set<TipoManutencaoCepModel>()
            .Where(t => t.TipoAtivo.ToLower().Contains(tipoAtivoNormalizado))
            .OrderBy(t => t.IdTipoManutencaoCep)
            .ToListAsync();
    }
}
