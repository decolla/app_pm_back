using app_pm_back.Data;
using app_pm_back.DTO.Buscas;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetDashboardDataAsync(FiltroDashboardDTO filtro)
    {
        List<NotaModel> notas = await GetNotasAsync(filtro);
        List<OrdemModel> ordens = await GetOrdensAsync(filtro);

        var relacaoStatusOrdens = RelacaoStatusOrdens(notas, notas.Count);

        return new
        {
        };
    }

    private async Task<List<NotaModel>> GetNotasAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.PM_Notas.AsQueryable();

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(n => n.IdAtivo == filtro.Ativo.Value);
        }

        if (filtro.Ultimos.HasValue && filtro.Ultimos.Value > 0)
        {
            var dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);
            query = query.Where(n => n.DataNota >= dataLimite);
        }

        if (filtro.NumeroRegistro.HasValue)
        {
            query = query.Where(n => n.IdNota == filtro.NumeroRegistro.Value);
        }

        if (!string.IsNullOrEmpty(filtro.IdLi))
        {
            query = query.Where(n => n.IdLi == filtro.IdLi);
        }

        return await query.ToListAsync();
    }

    private async Task<List<OrdemModel>> GetOrdensAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.PM_Ordens.AsQueryable();

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(n => n.IdAtivo == filtro.Ativo.Value);
        }

        if (filtro.Ultimos.HasValue && filtro.Ultimos.Value > 0)
        {
            var dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);
            query = query.Where(n => n.DataCriacao >= dataLimite);
        }

        if (filtro.NumeroRegistro.HasValue)
        {
            query = query.Where(n => n.IdOrdem == filtro.NumeroRegistro.Value);
        }

        if (!string.IsNullOrEmpty(filtro.IdLi))
        {
            query = query.Where(n => n.IdLi == filtro.IdLi);
        }

        return await query.ToListAsync();
    }

    private object RelacaoStatusOrdens(List<NotaModel> notas, int totalNotas)
    {
        int notasComOrdem = notas.Select(n => n.Ordens).Count(o => o.Count != 0);
        int notasSemOrdem = totalNotas - notasComOrdem;
        int notasRejeitadas = notas.Count(n => n.StatusNota == StatusFichas.Reprovada);
        int notasConcluidas = notas.Count(n => n.StatusNota == StatusFichas.Concluida);

        return Task.FromResult<object>(new
        {
            NotasSemOrdem = notasSemOrdem,
            NotasComOrdem = notasComOrdem,
            NotasRejeitadas = notasRejeitadas,
            NotasConcluidas = notasConcluidas
        });
    }

    private object NotasAbertasRelacaoPrazo(List<NotaModel> notas)
    {
        int notasComOrdemNoPrazo = notas
            .Select(n => n.Ordens)
            .Count(o => o.Count > 0 &&
                        o.Any(oo => oo.FimPlanejado > DateTime.Now));

        int notasSemOrdemNoPrazo = notas
            .Count(n => n.Ordens.Count == 0 &&
                        n.DataAvaria.HasValue &&
                        n.DataAvaria.Value < DateTime.Now);

        int notasSemOrdemAtrasadas = notas
            .Select(n => n.Ordens)
            .Count(o => o.Any(oo => oo.FimReal < DateTime.Now));

        int notasComOrdemAtrasadas = notas
            .Select(n => n.Ordens)
            .Count(o => o.Any(oo => oo.FimReal > DateTime.Now));


        return Task.FromResult<object>(new
        {
            NotasComOrdemNoPrazo = notasComOrdemNoPrazo,
            NotasSemOrdemNoPrazo = notasSemOrdemNoPrazo,
            NotasSemOrdemAtrasadas = notasSemOrdemAtrasadas,
            NotasComOrdemAtrasadas = notasComOrdemAtrasadas
        });
    }

    private Dictionary<(string, string), int> TopLiOcorrencias(List<NotaModel> notas, List<OrdemModel> ordens)
    {
        Dictionary<(string IdLi, DateTime? DataNota), int> liOcorrencias = new();

        foreach (var nota in notas)
        {
            if (nota.IdLi != null && nota.DataNota != null &&
                liOcorrencias.TryAdd(((string, DateTime))(nota.IdLi, nota.DataNota), 1))
            {
            }
            else if (nota.IdLi != null)
            {
                liOcorrencias[(nota.IdLi, nota.DataNota)]++;
            }
        }

        foreach (var ordem in ordens)
        {
            if (ordem.IdLi != null && ordem.DataCriacao != null &&
                liOcorrencias.TryAdd(((string, DateTime))(ordem.IdLi, ordem.DataCriacao), 1))
            {
            }
            else if (ordem.IdLi != null)
            {
                liOcorrencias[(ordem.IdLi, ordem.DataCriacao)]++;
            }
        }

        Dictionary<(string, string), int> liOcorrenciasFormatted = new();

        foreach (var liOcorrencia in liOcorrencias)
        {
            liOcorrenciasFormatted.Add(
                (liOcorrencia.Key.Item1, liOcorrencia.Key.Item2?.ToString("yyyy") ?? string.Empty), liOcorrencia.Value);
        }

        return liOcorrenciasFormatted.OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }
    
    // TODO: FINALIZAR DASHBOARD

}