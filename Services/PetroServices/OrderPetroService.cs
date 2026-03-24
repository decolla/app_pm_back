using app_pm_back.Data;
using app_pm_back.Models;

namespace app_pm_back.Services.PetroServices;

public class OrderPetroService : BaseCrudService<OrdemPetroModel>
{
    public OrderPetroService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota.
    /// </summary>
    /// <param name="ativo"></param>
    /// <param name="tipoOrdem"></param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(string ativo, string? tipoOrdem)
    {
        var query = _dbSet.AsQueryable(); // usa o _dbSet da Superclasse
        
        /*
        query = query.Where(n => n.LocInstalacao == ativo || n.Campo == ativo || n.Localizacao == ativo);
        
        if (!string.IsNullOrWhiteSpace(tipoNota))
        {
            query = query.Where(n => n.TipoNota == tipoNota);
        }
        
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.DataNota,
            Tipo = n.TipoNota
        }).ToListAsync();
        */
        
        return new
        {
            Ativo = ativo,
            Quantidade = 0,
            Fichas = 0
        };
    }
}