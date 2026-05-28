using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

public class TipoManutencaoCepService : BaseCrudService<TipoCepModel>
{
 public TipoManutencaoCepService(AppDbContext context) : base(context)
 {
 }

 public async Task<List<TipoCepModel>> BuscarPorTipoAtivoAsync(string tipoAtivo)
 {
 var tipoAtivoNormalizado = tipoAtivo.Trim().ToLower();

 return await _context.PM_TiposCep
 .Where(t => t.TipoAtivo.ToLower().Contains(tipoAtivoNormalizado))
 .OrderBy(t => t.IdTipoCep)
 .ToListAsync();
 }
}
