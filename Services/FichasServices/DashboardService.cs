using app_pm_back.Data;
using app_pm_back.DTO.Buscas;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

/// <summary>
/// Gera as estatísticas do dashboard 
/// </summary>
public class DashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Função principal que chamará todas as outras funções para gerar as estatísticas com base nas fichas após filtragem.
    /// </summary>
    /// <param name="filtro">Filtro para notas e ordens</param>
    /// <returns></returns>
    public async Task<object> GetDashboardDataAsync(FiltroDashboardDTO filtro)
    {
        // aplica a filtragem em notas e ordens
        List<NotaModel> notas = await GetNotasAsync(filtro);
        List<OrdemModel> ordens = await GetOrdensAsync(filtro);
        
        var statusSemana = RelacaoStatusOrdensSemana(notas);

        // retorna objeto com todas as informações do Dashboard
        return new
        {
            RelacaoStatusOrdens = RelacaoStatusOrdens(notas, notas.Count),
            NotasAbertasRelacaoPrazo = NotasAbertasRelacaoPrazo(notas),
            TopLiOcorrencias = TopLiOcorrencias(notas, ordens).ToList(),
            RelacaoStatusOrdensSemana = statusSemana.Item1,
            NotasAbertasRelacaoPrazoSemana = statusSemana.Item2,
            RankingTempoVida = RankingTempoVida(notas, ordens)
        };
    }

    /// <summary>
    /// Aplica filtragem nas notas 
    /// </summary>
    /// <param name="filtro">Filtro estabelecido</param>
    /// <returns>Notas filtradas</returns>
    private async Task<List<NotaModel>> GetNotasAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.PM_Notas
            .Include(n => n.Ordens)
            .AsNoTracking()
            .AsQueryable();

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(n => n.IdAtivo == filtro.Ativo.Value);
        }

        if (filtro.Ultimos.HasValue && filtro.Ultimos.Value > 0)
        {
            var dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);
            query = query.Where(n => n.DataNota >= dataLimite);
        }

        // if (filtro.NumeroRegistro.HasValue)
        // {
        //     query = query.Where(n => n.IdNota == filtro.NumeroRegistro.Value);
        // }

        if (!string.IsNullOrEmpty(filtro.IdLi))
        {
            query = query.Where(n => n.IdLi == filtro.IdLi);
        }

        // retorna notas filtradas
        return await query.ToListAsync();
    }

    /// <summary>
    /// Filtra ordens 
    /// </summary>
    /// <param name="filtro">Filtro a ser aplicado</param>
    /// <returns></returns>
    private async Task<List<OrdemModel>> GetOrdensAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.PM_Ordens
            .Include(o => o.Nota)
            .ThenInclude(n => n.Ordens)
            .AsNoTracking()
            .AsQueryable();

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(n => n.IdAtivo == filtro.Ativo.Value);
        }

        if (filtro.Ultimos.HasValue && filtro.Ultimos.Value > 0)
        {
            var dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);
            query = query.Where(n => n.DataCriacao >= dataLimite);
        }

        // if (filtro.NumeroRegistro.HasValue)
        // {
        //     query = query.Where(n => n.IdOrdem == filtro.NumeroRegistro.Value);
        // }

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

    private (List<object>, List<object>) RelacaoStatusOrdensSemana(List<NotaModel> notas)
    {
        if (!notas.Any(n => n.DataNota.HasValue)) return (new(), new());
        
        List<object> relacaoStatus = new();
        List<object> relacaoPrazo = new();

        var notasOrdenadas = notas.OrderBy(n => n.DataNota).ToList();

        DateTime? dataInicial = notasOrdenadas.Any()
            ? notasOrdenadas.FirstOrDefault(n => n.DataNota.HasValue)?.DataNota
            : DateTime.Now;

        int semana = 1;

        for (; dataInicial < DateTime.Now; dataInicial = dataInicial.Value.AddDays(7))
        {
            var notasSemana = notasOrdenadas
                .Where(n => n.DataNota >= dataInicial && n.DataNota < dataInicial.Value.AddDays(7)).ToList();
            int notasComOrdem = notasSemana.Select(n => n.Ordens).Count(o => o.Count != 0);
            int notasSemOrdem = notasSemana.Count() - notasComOrdem;
            int notasRejeitadas = notasSemana.Count(n => n.StatusNota == StatusFichas.Reprovada);
            int notasConcluidas = notasSemana.Count(n => n.StatusNota == StatusFichas.Concluida);

            relacaoStatus.Add(new
            {
                Semana = semana,
                NotasSemOrdem = notasSemOrdem,
                NotasComOrdem = notasComOrdem,
                NotasRejeitadas = notasRejeitadas,
                NotasConcluidas = notasConcluidas
            });

            int notasComOrdemPrazo = notasSemana.Count(n => n.Ordens.Any(o => o.FimPlanejado > DateTime.Now));
            int notasSemOrdemPrazo = notasSemana.Count(n =>
                n.Ordens.Count == 0 && n.DataAvaria.HasValue && n.FimDesejado < DateTime.Now);
            int notasSemOrdemAtrasadas = notasSemana.Count(n => n.Ordens.Any(o => o.FimReal < DateTime.Now));
            int notasComOrdemAtrasadas = notasSemana.Count(n => n.Ordens.Any(o => o.FimReal > DateTime.Now));

            relacaoPrazo.Add(new
            {
                Semana = semana,
                NotasSemOrdemPrazo = notasSemOrdemPrazo,
                NotasComOrdemPrazo = notasComOrdemPrazo,
                NotasSemOrdemAtrasadas = notasSemOrdemAtrasadas,
                NotasComOrdemAtrasadas = notasComOrdemAtrasadas
            });
            semana++;
        }

        return (relacaoStatus, relacaoPrazo);
    }

    private List<((long?, TimeSpan?), (long?, TimeSpan?))> RankingTempoVida(List<NotaModel> notas,
        List<OrdemModel> ordens)
    {
        var rankingTotal = new List<((long?, TimeSpan?), (long?, TimeSpan?))>();

        // processa todas notas e ordens 
        foreach (var nota in notas)
        {
            var ordemAssociada = nota.Ordens?.FirstOrDefault(o => o.FimReal.HasValue);

            TimeSpan? vidaNota = ordemAssociada?.FimReal - nota.DataNota;
            TimeSpan? vidaOrdem = ordemAssociada?.FimReal - ordemAssociada?.DataCriacao;

            rankingTotal.Add(((nota.IdNota, vidaNota), (ordemAssociada?.IdOrdem, vidaOrdem)));
        }

        // hash para não duplicar ordens já processadas
        var idsOrdensJaProcessadas = notas
            .SelectMany(n => n.Ordens ?? new List<OrdemModel>())
            .Select(o => o.IdOrdem)
            .ToHashSet();

        // processam ordens soltas
        foreach (var ordem in ordens)
        {
            if (idsOrdensJaProcessadas.Contains(ordem.IdOrdem))
                continue;

            var notaAssociada = ordem.Nota;

            TimeSpan? vidaOrdem = ordem.FimReal - ordem.DataCriacao;
            TimeSpan? vidaNota = notaAssociada?.Ordens?.FirstOrDefault(o => o.FimReal.HasValue)?.FimReal -
                                 notaAssociada?.DataNota;

            rankingTotal.Add(((notaAssociada?.IdNota, vidaNota), (ordem.IdOrdem, vidaOrdem)));
        }

        // limpa e ordena gerando um top 20
        var top = rankingTotal
            .Where(r => r.Item1.Item2.HasValue || r.Item2.Item2.HasValue)
            .OrderBy(r => r.Item1.Item2 ?? r.Item2.Item2 ?? TimeSpan.MaxValue)
            .Take(20) 
            .ToList();

        return top;
        
    }

}