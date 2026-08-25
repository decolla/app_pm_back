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
            TopLiOcorrencias = TopLiOcorrencias(notas, ordens)
                .Select(x => new { IdLi = x.Key.Item1, Ano = x.Key.Item2, Ocorrencias = x.Value })
                .ToList(),
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
    private Dictionary<(string, string), int> TopLiOcorrencias(List<NotaModel> notas, List<OrdemModel> ordens)
    {
        var liOcorrencias = new Dictionary<(string IdLi, string Ano), int>();

        // processa ocorrência das notas 
        void ProcessarOcorrencia(string? idLi, DateTime? data)
        {
            if (string.IsNullOrEmpty(idLi) || !data.HasValue) return;

            var chave = (idLi, data.Value.ToString("yyyy"));

            if (!liOcorrencias.TryAdd(chave, 1))
            {
                liOcorrencias[chave]++;
            }
        }

        // chama a função para cada nota e ordem
        foreach (var nota in notas) ProcessarOcorrencia(nota.IdLi, nota.DataNota);
        foreach (var ordem in ordens) ProcessarOcorrencia(ordem.IdLi, ordem.DataCriacao);

        // ordena as notas
        return liOcorrencias
            .OrderByDescending(x => x.Value)
            .Take(20)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Gera a relação de status de ordem e relação de prazo de notas
    /// </summary>
    /// <param name="notas"></param>
    /// <returns></returns>
    private (List<object>, List<object>) RelacaoStatusOrdensSemana(List<NotaModel> notas)
    {
        if (!notas.Any(n => n.DataNota.HasValue)) return (new(), new());

        List<object> relacaoStatus = new();
        List<object> relacaoPrazo = new();

        var notasOrdenadas = notas.OrderBy(n => n.DataNota).ToList();

        // obtém a data inicial com base na nota mais velha encontrada
        DateTime? dataInicial = notasOrdenadas.Any()
            ? notasOrdenadas.FirstOrDefault(n => n.DataNota.HasValue)?.DataNota
            : DateTime.Now;

        int semana = 1;

        // preenche os dados com base nas semanas
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
            TimeSpan? vidaNota = notaAssociada?.DataNota -
                                 notaAssociada?.Ordens?.FirstOrDefault(o => o.FimReal.HasValue)?.FimReal;

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