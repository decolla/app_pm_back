using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class TipoManutencaoService : BaseCrudService<TipoPlanoModel>
{
    public TipoManutencaoService(AppDbContext context) : base(context)
    {
        
    }
    
    // sobrescrevemos a busca por ID para incluir os filhos
    public async Task<TipoPlanoModel?> GetCompletoByIdAsync(int id)
    {
        return await _context.Set<TipoPlanoModel>()
            .Include(t => t.Ferramentas)
            .Include(t => t.ListaTarefa)
            .FirstOrDefaultAsync(t => t.IdTipoManutencao == id);
    }
    
    public async Task<List<TipoPlanoModel>> GetTodosByTipo(CategoriaTipoEstrategia tipo)
    {
        return await _context.Set<TipoPlanoModel>()
            .Where(t => t.Categoria == tipo)
            .ToListAsync();
    }

    public async Task<TipoPlanoModel> CreateComCopiaAsync(TipoPlanoModel novo)
    {
        if (novo.TipoEstrategiaId.HasValue)
        {
            var tipoEstrategia = await _context.Set<TipoEstrategiaModel>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTipoEstrategia == novo.TipoEstrategiaId.Value);

            if (tipoEstrategia == null)
            {
                throw new Exception("Tipo de estrategia nao encontrado.");
            }

            AplicarDadosTipoEstrategia(novo, tipoEstrategia);
        }

        return await CreateAsync(novo);
    }

    private static void AplicarDadosTipoEstrategia(TipoPlanoModel destino, TipoEstrategiaModel origem)
    {
        destino.Dias = origem.Dias;
        destino.ToleranciaPositiva = origem.ToleranciaPositiva;
        destino.ToleranciaNegativa = origem.ToleranciaNegativa;
        destino.Continua = origem.Continua;
        destino.Categoria = origem.Categoria;
        destino.TipoManutencaoCepId = origem.IdTipoManutencaoCep;
        destino.TipoManutencaoId = origem.IdTipoManutencao;
    }

    // método especializado para atualizar o "Grafo" de objetos
    public async Task<TipoPlanoModel> UpdateComListasAsync(TipoPlanoModel atualizado)
    {
        var existente = await GetCompletoByIdAsync(atualizado.IdTipoManutencao);
        if (existente == null) throw new Exception("Registo não encontrado");

        // Atualiza campos do pai
        _context.Entry(existente).CurrentValues.SetValues(atualizado);

        // Atualiza Ferramentas (Substitui a lista)
        existente.Ferramentas!.Clear();
        foreach (var f in atualizado.Ferramentas ?? new List<FerramentasTipoModel>()) existente.Ferramentas.Add(f);


        // Atualiza Tarefas (Substitui a lista)
        existente.ListaTarefa!.Clear();
        foreach (var t in atualizado.ListaTarefa ?? new List<TarefaModel>()) existente.ListaTarefa.Add(t);

        await _context.SaveChangesAsync();
        return existente;
    }
    
}