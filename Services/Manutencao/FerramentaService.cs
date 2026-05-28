using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app_pm_back.Services.Manutencao;

public class FerramentaService : BaseCrudService<FerramentaModel>
{
 public FerramentaService(AppDbContext context) : base(context)
 {
 }

 public async Task<List<FerramentaModel>> BuscarPorTipoManutencaoAsync(int tipoManutencaoId)
 {
 return await _context.Set<FerramentaModel>()
 .Where(f => f.Operacoes.Any(o => o.IdTipoManutencao == tipoManutencaoId))
 .OrderBy(f => f.IdFerramenta)
 .ToListAsync();
 }
}
