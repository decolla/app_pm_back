using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;


namespace app_pm_back.Services;

public class NaturezasCausaService : BaseCrudService<NaturezasCausaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NaturezasCausaService"/>.
    /// </summary>
    public NaturezasCausaService(AppDbContext context) : base(context)
    {
        
    }
    
    /// <summary>
    /// Busca todas as naturezas da causa com base no tipo de classificação (Corretiva ou Ocorrência Alheia).
    /// </summary>
    /// <param name="ehCorretiva">True para buscar Corretivas, False para Ocorrências Alheias.</param>
    /// <returns>Lista de naturezas da causa filtrada.</returns>
    public async Task<List<NaturezasCausaModel>> ObterPorClassificacaoAsync(bool ehCorretiva)
    {
        return await _dbSet
            .Where(n => n.EhCorretiva == ehCorretiva)
            .ToListAsync();
    }
}