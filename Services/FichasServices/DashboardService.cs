using System.Runtime.InteropServices.JavaScript;
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
            TopLiOcorrencias = TopLiOcorrencias(notas, ordens),
            RelacaoStatusOrdensSemana = statusSemana.Item1,
            NotasAbertasRelacaoPrazoSemana = statusSemana.Item2,
            RankingTempoVida = RankingTempoVida(notas, ordens)
                .Select(x => new
                {
                    Nota = new { IdNota = x.Item1.Item1, VidaNota = x.Item1.Item2 },
                    Ordem = new { IdOrdem = x.Item2.Item1, VidaOrdem = x.Item2.Item2 }
                })
                .ToList()
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
            DateTime dataLimite;
            if (filtro.UnidadeTempo == UnidadeTempo.Meses)
                dataLimite = DateTime.Now.AddMonths(-filtro.Ultimos.Value);
            else if (filtro.UnidadeTempo == UnidadeTempo.Anos)
                dataLimite = DateTime.Now.AddYears(-filtro.Ultimos.Value);
            else
                dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);

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
    /// <returns>Ordens filtradas</returns>
    private async Task<List<OrdemModel>> GetOrdensAsync(FiltroDashboardDTO filtro)
    {
        var query = _context.PM_Ordens
            .Include(o => o.Nota)
            .ThenInclude(n => n.Ordens)
            .AsQueryable();

        if (filtro.Ativo.HasValue)
        {
            query = query.Where(n => n.IdAtivo == filtro.Ativo.Value);
        }

        if (filtro.Ultimos.HasValue && filtro.Ultimos.Value > 0)
        {
            DateTime dataLimite;
            if (filtro.UnidadeTempo == UnidadeTempo.Meses)
                dataLimite = DateTime.Now.AddMonths(-filtro.Ultimos.Value);
            else if (filtro.UnidadeTempo == UnidadeTempo.Anos)
                dataLimite = DateTime.Now.AddYears(-filtro.Ultimos.Value);
            else
                dataLimite = DateTime.Now.AddDays(-filtro.Ultimos.Value);

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

    /// <summary>
    /// Estatísticas de status de notas 
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="totalNotas"></param>
    /// <returns></returns>
    private object RelacaoStatusOrdens(List<NotaModel> notas, int totalNotas)
    {
        int notasComOrdem = notas.Count(n => n.Ordens != null && n.Ordens.Any());
        int notasSemOrdem = totalNotas - notasComOrdem;
        int notasRejeitadas = notas.Count(n => n.StatusNota == StatusFichas.Reprovada);
        int notasConcluidas = notas.Count(n => n.StatusNota == StatusFichas.Concluida);

        return new
        {
            NotasSemOrdem = notasSemOrdem,
            NotasComOrdem = notasComOrdem,
            NotasRejeitadas = notasRejeitadas,
            NotasConcluidas = notasConcluidas
        };
    }

    /// <summary>
    /// Estatísticas de notas abertas com relação ao seu prazo
    /// </summary>
    /// <param name="notas"></param>
    /// <returns></returns>
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

        return new
        {
            NotasComOrdemNoPrazo = notasComOrdemNoPrazo,
            NotasSemOrdemNoPrazo = notasSemOrdemNoPrazo,
            NotasSemOrdemAtrasadas = notasSemOrdemAtrasadas,
            NotasComOrdemAtrasadas = notasComOrdemAtrasadas
        };
    }

    /// <summary>
    /// Estatísticas de top LI por ocorrência
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="ordens"></param>
    /// <returns></returns>
    private Dictionary<string, (int notas, int ordens)> TopLiOcorrencias(List<NotaModel> notas, List<OrdemModel> ordens)
    {
        var liOcorrencias = new Dictionary<string, (int notas, int ordens)>();

        // processa ocorrência das fichas 
        void ProcessarOcorrencia(string? idLi, bool ehNota)
        {
            if (!string.IsNullOrEmpty(idLi))
                return;

            if (!liOcorrencias.TryGetValue(idLi, out var ocorrencia))
            {
                ocorrencia = (0, 0);
            }

            if (ehNota)
                ocorrencia.notas++;
            else
                ocorrencia.ordens++;

            liOcorrencias[idLi] = ocorrencia;
        }

        // chama a função para cada nota e ordem
        foreach (var nota in notas) ProcessarOcorrencia(nota.IdLi, true);
        foreach (var nota in notas) ProcessarOcorrencia(nota.IdAtivo.ToString(), true);
        foreach (var ordem in ordens) ProcessarOcorrencia(ordem.IdLi, false);
        foreach (var ordem in ordens) ProcessarOcorrencia(ordem.IdAtivo.ToString(), false);

        // ordena as notas
        return liOcorrencias
            .OrderByDescending(x => x.Value)
            .Take(7)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Gera a relação de status de ordem e relação de prazo de notas
    /// </summary>
    /// <param name="notas"></param>
    /// <returns></returns>
    private (List<object>, List<object>) RelacaoStatusOrdensSemana(List<NotaModel> notas)
    {
        var notasComData = notas.Where(n => n.DataNota.HasValue).ToList();
        if (!notasComData.Any()) return (new(), new());

        List<object> relacaoStatus = new();
        List<object> relacaoPrazo = new();

        var notasOrdenadas = notasComData.OrderBy(n => n.DataNota).ToList();
        DateTime dataInicial = notasOrdenadas.First().DataNota!.Value;

        // agrupa as notas por semana
        var notasPorSemana = notasOrdenadas
            .GroupBy(n => (int)((n.DataNota!.Value - dataInicial).TotalDays / 7) + 1)
            .ToDictionary(g => g.Key, g => g.ToList());

        int totalSemanas = (int)((DateTime.Now - dataInicial).TotalDays / 7) + 1;

        for (int semana = 1; semana <= totalSemanas; semana++)
        {
            if (!notasPorSemana.TryGetValue(semana, out var notasSemana) || notasSemana.Count == 0)
            {
                // se a série for muito longa
                if (totalSemanas > 52)
                    continue;

                notasSemana = new List<NotaModel>();
            }

            int notasComOrdem = notasSemana.Select(n => n.Ordens).Count(o => o.Count != 0);
            int notasSemOrdem = notasSemana.Count - notasComOrdem;
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
        }

        return (relacaoStatus, relacaoPrazo);
    }

    /// <summary>
    /// Ranking de tempo de vida das notas e ordens
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="ordens"></param>
    /// <returns></returns>
    private List<((long?, TimeSpan?), (long?, TimeSpan?))> RankingTempoVida(List<NotaModel> notas,
        List<OrdemModel> ordens)
    {
        var rankingTotal = new List<((long?, TimeSpan?), (long?, TimeSpan?))>();

        // processa todas notas e ordens 
        foreach (var nota in notas)
        {
            if (nota.StatusNota == StatusFichas.Concluida) continue;

            var ordemAssociada = nota.Ordens?.FirstOrDefault(o => o.FimReal.HasValue);

            TimeSpan? vidaNota = DateTime.UtcNow - nota.DataNota;
            TimeSpan? vidaOrdem = ordemAssociada?.DataCriacao != null
                ? DateTime.UtcNow - ordemAssociada?.DataCriacao
                : (TimeSpan?)null;

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
            if (idsOrdensJaProcessadas.Contains(ordem.IdOrdem) || ordem.StatusOrdem == StatusFichas.Concluida)
                continue;

            var notaAssociada = ordem.Nota;

            TimeSpan? vidaOrdem = DateTime.UtcNow - ordem.DataCriacao;
            TimeSpan? vidaNota = notaAssociada?.DataNota != null
                ? DateTime.UtcNow - notaAssociada?.DataNota
                : (TimeSpan?)null;

            rankingTotal.Add(((notaAssociada?.IdNota, vidaNota), (ordem.IdOrdem, vidaOrdem)));
        }

        // limpa e ordena gerando um top 20
        var top = rankingTotal
            .Where(r => r.Item1.Item2.HasValue || r.Item2.Item2.HasValue)
            .OrderBy(r => r.Item1.Item2 ?? r.Item2.Item2 ?? TimeSpan.MaxValue)
            .Take(7)
            .ToList();

        return top;
    }
}