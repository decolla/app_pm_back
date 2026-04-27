using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class TipoManutencaoService : BaseCrudService<TipoManutencaoModel>
{
    public TipoManutencaoService(AppDbContext context) : base(context)
    {
        
    }
    
    // sobrescrevemos a busca por ID para incluir os filhos
    public async Task<TipoManutencaoModel?> GetCompletoByIdAsync(int id)
    {
        return await _context.Set<TipoManutencaoModel>()
            .Include(t => t.Ferramentas)
            .Include(t => t.ItensRenovacao)
            .Include(t => t.ListaTarefa)
            .FirstOrDefaultAsync(t => t.IdTipoManutencao == id);
    }
    
    public async Task<List<TipoManutencaoModel>> GetTodosByTipo(TipoTipo tipo)
    {
        return await _context.Set<TipoManutencaoModel>()
            .Where(t => t.TipoTipo == tipo)
            .ToListAsync();
    }

    // método especializado para atualizar o "Grafo" de objetos
    public async Task<TipoManutencaoModel> UpdateComListasAsync(TipoManutencaoModel atualizado)
    {
        var existente = await GetCompletoByIdAsync(atualizado.IdTipoManutencao);
        if (existente == null) throw new Exception("Registo não encontrado");

        // Atualiza campos do pai
        _context.Entry(existente).CurrentValues.SetValues(atualizado);

        // Atualiza Ferramentas (Substitui a lista)
        existente.Ferramentas!.Clear();
        foreach (var f in atualizado.Ferramentas ?? new List<FerramentasTipoModel>()) existente.Ferramentas.Add(f);

        // Atualiza Itens Renovação (Substitui a lista)
        existente.ItensRenovacao!.Clear();
        foreach (var i in atualizado.ItensRenovacao ?? new List<ItemRenovacaoTipoModel>()) existente.ItensRenovacao.Add(i);

        // Atualiza Tarefas (Substitui a lista)
        existente.ListaTarefa!.Clear();
        foreach (var t in atualizado.ListaTarefa ?? new List<TarefaModel>()) existente.ListaTarefa.Add(t);

        await _context.SaveChangesAsync();
        return existente;
    }
    
}