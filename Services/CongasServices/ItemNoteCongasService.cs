using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class ItemNoteCongasService : BaseCrudService<ItemNotaManutencaoModel>
{
    public ItemNoteCongasService(AppDbContext context) : base(context)
    {
        
    }

    /// <summary>
    /// Importa itens em lote, criando notas fantasma quando necessario.
    /// </summary>
    public async Task ImportarItensEmLoteSeguroAsync(List<ItemNotaManutencaoModel> itens)
    {
        if (itens == null || !itens.Any()) return;

        var notaIds = itens.Select(i => (long?)i.NotaId).Distinct().ToList();
        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds);

        var idsItens = itens.Select(i => i.IdItemNota).Distinct().ToList();
        var existentes = await _context.PM_ItensNotaManutencao
            .Where(i => idsItens.Contains(i.IdItemNota))
            .ToDictionaryAsync(i => i.IdItemNota);

        var novos = new List<ItemNotaManutencaoModel>();
        foreach (var item in itens)
        {
            if (existentes.TryGetValue(item.IdItemNota, out var existente))
            {
                ImportMergeHelper.MergeNonEmpty(existente, item);
            }
            else
            {
                novos.Add(item);
            }
        }

        if (novos.Any())
        {
            await _context.PM_ItensNotaManutencao.AddRangeAsync(novos);
        }

        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Retorna os itens de uma nota pelo ID
    /// </summary>
    /// <param name="id">Id da nota a ser realizada a busca</param>
    /// <returns>Lista de Itens associadas a essa nota</returns>
    public async Task<List<ItemNotaManutencaoModel>> GetItemsByNotaIdAsync(long id) // USE long ou int AQUI!
    {
        return await _dbSet.Where(a => a.NotaId == id).ToListAsync();
    }
}