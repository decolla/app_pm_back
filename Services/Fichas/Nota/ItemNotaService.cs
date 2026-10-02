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
    public virtual async Task ImportarItensEmLoteSeguroAsync(List<ItemNotaModel>? itens,
        CancellationToken cancellationToken)
    {
        if (itens == null || !itens.Any()) return;

        // garante que as notas existam
        var notaIds = itens.Select(i => i.IdNota).Distinct().ToList();
        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds.Cast<long?>().ToList());

        // busca pelos itens já existentes
        var itensExistentes = await _context.PM_ItensNota
            .Where(i => notaIds.Contains(i.IdNota))
            .ToListAsync(cancellationToken);

        var novos = new List<ItemNotaModel>();
        foreach (var itemTela in itens)
        {
            // verifica a existência de notas com os mesmos dados
            var existente = itensExistentes.FirstOrDefault(e => 
                e.IdNota == itemTela.IdNota && 
                e.Componente == itemTela.Componente && 
                e.Sintoma == itemTela.Sintoma && 
                e.Causa == itemTela.Causa);
            
            if (existente != null)
            {
                // caso de item existente
                ImportMergeHelper.MergeNonEmpty(existente, itemTela);
            }
            else
            {
                // caso de item novo
                novos.Add(itemTela);
                itensExistentes.Add(itemTela);
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