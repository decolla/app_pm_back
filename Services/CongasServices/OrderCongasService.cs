using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class OrderCongasService : BaseCrudService<OrdemManutencaoModel>
{
    public OrderCongasService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota.
    /// </summary>
    /// <param name="ativo"></param>
    /// <param name="tipoOrdem"></param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(int ativo, string? tipoOrdem)
    {
        var query = _dbSet.AsQueryable(); // usa o _dbSet da Superclasse
        
        query = query.Where(n => 
            n.NotaRelacionada.LocalInstalacaoCodigo == ativo);
        
        if (!string.IsNullOrWhiteSpace(tipoOrdem))
        {
            query = query.Where(n => n.TipoOrdem == tipoOrdem);
        }
        
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.NotaRelacionadaId,
            Data = n.DataEntrada,
            Tipo = n.TipoOrdem
        }).ToListAsync();
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}