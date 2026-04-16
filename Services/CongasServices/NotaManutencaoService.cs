using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class NotaManutencaoService : BaseCrudService<NotaManutencaoModel>
{
    public NotaManutencaoService(AppDbContext context) : base(context)
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
        
        query = query.Where(n => n.LocalInstalacaoCodigo == ativo || n.EquipamentoTag == ativo);
        
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