using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;
/// <summary>
/// Classe OrdemService.
/// </summary>

public class OrdemService : BaseCrudService<OrdemModel>
{
 /// <summary>
 /// Inicializa uma nova instância da classe <see cref="OrdemService"/>.
 /// </summary>
 public OrdemService(AppDbContext context) : base(context)
 {
 }
 /// <summary>
 /// Executa a ação correspondente.
 /// </summary>
 /// <param name="ordens">Parâmetro do método.</param>

 public async Task ImportarOrdensEmLoteSeguroAsync(List<OrdemModel> ordens)
 {
 if (ordens == null || !ordens.Any()) return;

 var notaIds = ordens
 .Where(o => o.IdNota.HasValue)
 .Select(o => o.IdNota!.Value)
 .Distinct()
 .ToList();

 await ImportGhostHelper.EnsureNotasAsync(_context, notaIds.Select(id => (long?)id));

 var idsOrdens = ordens.Select(o => o.IdOrdem).Distinct().ToList();
 var existentes = await _context.PM_Ordens
 .Where(o => idsOrdens.Contains(o.IdOrdem))
 .ToDictionaryAsync(o => o.IdOrdem);

 var novasOrdens = new List<OrdemModel>();
 foreach (var ordem in ordens)
 {
 if (existentes.TryGetValue(ordem.IdOrdem, out var existente))
 {
 ImportMergeHelper.MergeNonEmpty(existente, ordem);
 }
 else
 {
 novasOrdens.Add(ordem);
 }
 }

 if (novasOrdens.Any())
 {
 await _context.PM_Ordens.AddRangeAsync(novasOrdens);
 }

 await _context.SaveChangesAsync();
 }
 /// <summary>
 /// Obtém um registro específico.
 /// </summary>
 /// <param name="ativo">Parâmetro do método.</param>
 /// <returns>Resultado da operação.</returns>
 
 public async Task<object> GetFichasPorAtivoAsync(int ativo, string? tipoOrdem)
 {
 var query = _dbSet.AsQueryable();
 
 query = query.Where(n => n.Nota != null && n.Nota.IdAtivo == ativo);
 
 var fichas = await query.Select(n => new
 {
 NumerFicha = n.IdNota,
 Data = n.FimReal ?? n.FimPlanejado ?? DateTime.UtcNow,
 Tipo = n.Classificacao
 }).ToListAsync();
 
 return new
 {
 Ativo = ativo,
 Quantidade = fichas.Count,
 Fichas = fichas
 };
 }
}