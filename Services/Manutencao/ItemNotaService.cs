using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe ItemNotaService.
/// </summary>
public class ItemNotaService : BaseCrudService<ItemNotaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ItemNotaService"/>.
    /// </summary>
    public ItemNotaService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="itens">Parâmetro do método.</param>
    public async Task ImportarItensEmLoteSeguroAsync(List<ItemNotaModel> itens)
    {
        if (itens == null || !itens.Any()) return;

        var notaIds = itens.Select(i => (long?)i.IdNota).Distinct().ToList();
        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds);

        var idsItens = itens.Select(i => i.IdItemNota).Distinct().ToList();
        var existentes = await _context.PM_ItensNota
            .Where(i => idsItens.Contains(i.IdItemNota))
            .ToDictionaryAsync(i => i.IdItemNota);

        var novos = new List<ItemNotaModel>();
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
            await _context.PM_ItensNota.AddRangeAsync(novos);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<ItemNotaModel>> GetItemsByNotaIdAsync(long id)
    {
        return await _dbSet.Where(a => a.IdNota == id).ToListAsync();
    }
}