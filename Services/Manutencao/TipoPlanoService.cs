using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

public class TipoPlanoService : BaseCrudService<TipoPlanoModel>
{
 public TipoPlanoService(AppDbContext context) : base(context)
 {
 }
 
 public async Task<TipoPlanoModel?> GetCompletoByIdAsync(int id)
 {
 return await _context.PM_TiposPlano
 .Include(t => t.TipoEstrategia)
 .Include(t => t.TipoCep)
 .Include(t => t.TipoManutencao)
 .FirstOrDefaultAsync(t => t.IdTipoPlano == id);
 }
 
 public async Task<List<TipoPlanoModel>> GetTodosByTipo(string categoria)
 {
 return await _context.PM_TiposPlano
 .Where(t => t.Categoria == categoria)
 .ToListAsync();
 }

 public async Task<TipoPlanoModel> CreateComCopiaAsync(TipoPlanoModel novo)
 {
 var tipoEstrategia = await _context.PM_TiposEstrategia
 .AsNoTracking()
 .FirstOrDefaultAsync(t => t.IdTipoEstrategia == novo.IdTipoEstrategia);

 if (tipoEstrategia == null)
 {
 throw new Exception("Tipo de estrategia nao encontrado.");
 }

 AplicarDadosTipoEstrategia(novo, tipoEstrategia);

 return await CreateAsync(novo);
 }

 private static void AplicarDadosTipoEstrategia(TipoPlanoModel destino, TipoEstrategiaModel origem)
 {
 destino.Dias = origem.Dias;
 destino.ToleranciaPositiva = origem.ToleranciaPositiva;
 destino.ToleranciaNegativa = origem.ToleranciaNegativa;
 destino.Continua = origem.Continua;
 destino.Categoria = origem.Categoria;
 destino.IdTipoCep = origem.IdTipoCep;
 destino.IdTipoManutencao = origem.IdTipoManutencao;
 }

 public async Task<TipoPlanoModel> UpdateComListasAsync(TipoPlanoModel atualizado)
 {
 var existente = await GetCompletoByIdAsync(atualizado.IdTipoPlano);
 if (existente == null) throw new Exception("Registo não encontrado");

 _context.Entry(existente).CurrentValues.SetValues(atualizado);
 await _context.SaveChangesAsync();
 return existente;
 }
}
