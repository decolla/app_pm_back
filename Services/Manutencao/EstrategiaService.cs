using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;
/// <summary>
/// Classe EstrategiaService.
/// </summary>

public class EstrategiaService : BaseCrudService<EstrategiaModel>
{
 /// <summary>
 /// Inicializa uma nova instância da classe <see cref="EstrategiaService"/>.
 /// </summary>
 public EstrategiaService(AppDbContext context) : base(context)
 {
 }
 /// <summary>
 /// Obtém um registro específico.
 /// </summary>
 /// <returns>Resultado da operação.</returns>

 public async Task<List<EstrategiaModel>> BuscarAsync(
 int? id,
 string? nome,
 string? tipoAtivo,
 string? visao,
 int? pagina,
 int? tamanho)
 {
 var query = _context.PM_Estrategias
 .AsNoTracking()
 .AsQueryable();

 if (id.HasValue)
 {
 query = query.Where(e => e.IdEstrategia == id.Value);
 }

 if (!string.IsNullOrWhiteSpace(nome))
 {
 var nomeNormalizado = nome.Trim().ToLower();
 query = query.Where(e => e.Nome.ToLower().Contains(nomeNormalizado));
 }

 if (!string.IsNullOrWhiteSpace(tipoAtivo))
 {
 var tipoAtivoNormalizado = tipoAtivo.Trim().ToLower();
 query = query.Where(e => e.TipoAtivoAplicavel.ToLower().Contains(tipoAtivoNormalizado));
 }

 query = query.OrderBy(e => e.IdEstrategia);

 if (pagina.HasValue || tamanho.HasValue)
 {
 var p = pagina ?? 1;
 var t = tamanho ?? 20;
 query = query.Skip((p - 1) * t).Take(t);
 }

 return await query.ToListAsync();
 }
}