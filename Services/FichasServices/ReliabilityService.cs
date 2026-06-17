using app_pm_back.Data;
using app_pm_back.Models.Domain;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.DTO;

public class ReliabilityService
{
    private readonly AppDbContext _context;
    private readonly DbSet<NotaModel> _notaModels;
    private readonly DbSet<OrdemModel> _ordemModels;
    
    public ReliabilityService(AppDbContext context)
    {
        _context = context;
        _notaModels = context.PM_Notas;
        _ordemModels = context.PM_Ordens;
    }
    
    public async Task<object?> DatasReliability(string idLi)
    {
        //Busca o registro de confiabilidade base para pegar as listas de IDs
        var reliability = await _context.PM_Reliability
            .AsNoTracking() 
            .FirstOrDefaultAsync(r => r.IdLi == idLi);

        //Variáveis para guardar as datas (inicializam vazias)
        List<DateTime> datasNotas = new List<DateTime>();
        List<DateTime> datasOrdens = new List<DateTime>();

        //Se achou o Li, processa as Notas apenas se a lista tiver IDs
        if (reliability?.IdsNotas != null && reliability.IdsNotas.Any())
        {
            datasNotas = await _notaModels.AsNoTracking()
                .Where(n => reliability.IdsNotas.Contains(n.IdNota) && n.DataNota.HasValue)
                .Select(n => n.DataNota!.Value)
                .ToListAsync();
        }

        //Processa as Ordens apenas se a lista tiver IDs
        if (reliability?.IdsOrdens != null && reliability.IdsOrdens.Any())
        {
            datasOrdens = await _ordemModels.AsNoTracking()
                .Where(o => reliability.IdsOrdens.Contains(o.IdOrdem) && o.DataCriacao.HasValue)
                .Select(o => o.DataCriacao!.Value)
                .ToListAsync();
        }

        //Junta tudo, remove duplicadas e ordena cronologicamente
        var todasDatas = datasNotas
            .Concat(datasOrdens)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        return new
        {
            SapName = idLi,
            Datas = todasDatas
        };
    }
}