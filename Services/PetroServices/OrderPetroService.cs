using app_pm_back.Data;
using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

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
        var query = _dbSet
            .Include(o => o.NotaOrigem)
            .Where(o => o.NotaOrigem != null)
            .Where(o => o.NotaOrigem!.LocInstalacao == ativo ||
                        o.NotaOrigem.Campo == ativo ||
                        o.NotaOrigem.Localizacao == ativo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tipoOrdem))
        {
            query = query.Where(o => o.Status == tipoOrdem);
        }

        var fichas = await query.Select(o => new
        {
            NumerFicha = o.IdOrdem,
            Data = o.DataCriacao,
            Tipo = o.Status
        }).ToListAsync();
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}