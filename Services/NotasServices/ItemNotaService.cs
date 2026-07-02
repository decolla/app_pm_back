using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe ItemNotaService.
/// </summary>
public class ItemNotaService : BaseCrudService<ItemNotaModel>
{
    private readonly DbSet<NotaModel> _notaModels;
    private readonly DbSet<ItemNotaModel> _itemNotaModels;
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ItemNotaService"/>.
    /// </summary>
    public ItemNotaService(AppDbContext context) : base(context)
    {
        _notaModels = context.PM_Notas;
        _itemNotaModels = context.PM_ItensNota;
    }
    
    public override async Task<ItemNotaModel> CreateAsync(ItemNotaModel itemModel)
    {
        //verifica se a nota existe
        var nota = await _notaModels
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.IdNota == itemModel.IdNota);

        if (nota == null)
            throw new InvalidOperationException($"Nota com Id {itemModel.IdNota} não encontrada.");

        // Se existir, isso já cadastra na tabela de itens de nota e coloca o item de nota na lista de notas
        nota.Itens.Add(itemModel);
        await _context.SaveChangesAsync();

        return itemModel;
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="itens">Parâmetro do método.</param>
    public virtual async Task ImportarItensEmLoteSeguroAsync(List<ItemNotaModel> itens,
        CancellationToken cancellationToken)
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
            cancellationToken.ThrowIfCancellationRequested();
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