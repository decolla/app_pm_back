using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

public class NotaManutencaoService : BaseCrudService<NotaModel>
{
 public NotaManutencaoService(AppDbContext context) : base(context)
 {
 }
 
 public async Task ImportarNotasEmLoteSeguroAsync(List<NotaModel> notas)
 {
 if (notas == null || !notas.Any()) return;

 var usuariosCriadoresExcel = notas
 .Where(n => n.IdUsuario.HasValue)
 .Select(n => n.IdUsuario!.Value)
 .Distinct()
 .ToList();

 var planosExcel = notas
 .Where(n => n.IdPlano.HasValue)
 .Select(n => n.IdPlano!.Value)
 .Distinct()
 .ToList();

 await ImportGhostHelper.EnsureUsuariosSapAsync(_context, usuariosCriadoresExcel.Select(id => (int?)id));
 await ImportGhostHelper.EnsurePlanosAsync(_context, planosExcel.Select(id => (long?)id));

 var idsNotas = notas.Select(n => n.IdNota).Distinct().ToList();
 var numerosNotas = notas.Select(n => n.NumeroNota).Distinct().ToList();

 var existentesPorId = await _context.PM_Notas
 .Where(n => idsNotas.Contains(n.IdNota))
 .ToDictionaryAsync(n => n.IdNota);

 var existentesPorNumero = await _context.PM_Notas
 .Where(n => numerosNotas.Contains(n.NumeroNota))
 .ToDictionaryAsync(n => n.NumeroNota);

 var novasNotas = new List<NotaModel>();
 foreach (var nota in notas)
 {
 if (existentesPorId.TryGetValue(nota.IdNota, out var existente) ||
 existentesPorNumero.TryGetValue(nota.NumeroNota, out existente))
 {
 ImportMergeHelper.MergeNonEmpty(existente, nota);
 }
 else
 {
 novasNotas.Add(nota);
 }
 }

 if (novasNotas.Any())
 {
 await _dbSet.AddRangeAsync(novasNotas);
 }

 await _context.SaveChangesAsync();
 }

 public async Task<object> GetFichasPorAtivoAsync(int ativo, string? tipoNota)
 {
 var query = _dbSet.AsQueryable(); 
 
 query = query.Where(n => n.IdAtivo == ativo);
 
 var fichas = await query.Select(n => new
 {
 NumerFicha = n.IdNota,
 Data = n.DataNota,
 Tipo = "MG" // default or mock value
 }).ToListAsync();
 
 return new
 {
 Ativo = ativo,
 Quantidade = fichas.Count,
 Fichas = fichas
 };
 }
}