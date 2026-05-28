using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

public class TipoManutencaoService : BaseCrudService<TipoManutencaoModel>
{
 public TipoManutencaoService(AppDbContext context) : base(context)
 {
 }

 public async Task<TipoManutencaoModel?> GetCompletoByIdAsync(int id)
 {
 return await _context.PM_TiposManutencao
 .FirstOrDefaultAsync(t => t.IdTipoManutencao == id);
 }

 public async Task<TipoManutencaoModel> UpdateBasicoAsync(TipoManutencaoModel atualizado)
 {
 var existente = await _context.PM_TiposManutencao
 .FirstOrDefaultAsync(t => t.IdTipoManutencao == atualizado.IdTipoManutencao);

 if (existente == null)
 {
 throw new Exception("Registo não encontrado");
 }

 existente.Categoria = atualizado.Categoria;
 existente.Nome = atualizado.Nome;
 existente.TipoAtivo = atualizado.TipoAtivo;
 existente.Confiabilidade = atualizado.Confiabilidade;
 existente.Habilitada = atualizado.Habilitada;

 await _context.SaveChangesAsync();
 return existente;
 }
}