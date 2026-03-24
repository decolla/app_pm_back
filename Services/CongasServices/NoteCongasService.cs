using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class NoteCongasService : BaseCrudService<NoteCongasModel>
{
    public NoteCongasService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota
    /// </summary>
    /// <param name="ativo"></param>
    /// <param name="tipoNota"></param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(string ativo, string? tipoNota)
    {
        var query = _dbSet.AsQueryable(); 
        
        query = query.Where(n => n.LocInstalacao == ativo || n.Denominacao == ativo || n.Local == ativo);
        
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
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}