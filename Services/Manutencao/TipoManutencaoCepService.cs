using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;
/// <summary>
/// Classe TipoManutencaoCepService.
/// </summary>

public class TipoManutencaoCepService : BaseCrudService<TipoCepModel>
{
 /// <summary>
 /// Inicializa uma nova instância da classe <see cref="TipoManutencaoCepService"/>.
 /// </summary>
 public TipoManutencaoCepService(AppDbContext context) : base(context)
 {
 }
 /// <summary>
 /// Obtém um registro específico.
 /// </summary>
 /// <param name="tipoAtivo">Parâmetro do método.</param>
 /// <returns>Resultado da operação.</returns>

 public async Task<List<TipoCepModel>> BuscarPorTipoAtivoAsync(string tipoAtivo)
 {
 var tipoAtivoNormalizado = tipoAtivo.Trim().ToLower();

 return await _context.PM_TiposCep
 .Where(t => t.TipoAtivo.ToLower().Contains(tipoAtivoNormalizado))
 .OrderBy(t => t.IdTipoCep)
 .ToListAsync();
 }
}
