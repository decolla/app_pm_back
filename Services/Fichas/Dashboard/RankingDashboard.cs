using app_pm_back.Models.Domain;
using app_pm_back.Services.Fichas.Dashboard;

namespace app_pm_back.Services.FichasServices;

public class RankingDashboard
{

    public RankingDashboard()
    {
    }

    /// <summary>
    /// Estatísticas de top LI por ocorrência
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="ordens"></param>
    /// <returns></returns>
    public Dictionary<string, (int notas, int ordens)> TopLiOcorrencias(List<NotaModel> notas, List<OrdemModel> ordens)
    {
        var liOcorrencias = new Dictionary<string, (int notas, int ordens)>();

        // processa ocorrência das fichas 
        void ProcessarOcorrencia(string? idLi, bool ehNota)
        {
            if (string.IsNullOrEmpty(idLi))
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
        foreach (var ordem in ordens) ProcessarOcorrencia(ordem.IdLi, false);
        

        // ordena as notas
        return liOcorrencias
            .OrderByDescending(x => x.Value)
            .Take(7)
            .ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Ranking de tempo de vida das notas e ordens
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="ordens"></param>
    /// <returns></returns>
    public List<((long?, TimeSpan?), (long?, TimeSpan?))> RankingTempoVida(List<NotaModel> notas,
        List<OrdemModel> ordens)
    {
        var rankingTotal = new List<((long?, TimeSpan?), (long?, TimeSpan?))>();

        // processa todas notas e ordens 
        foreach (var nota in notas)
        {
            if (nota.StatusNota == StatusFichas.Concluida) continue;

            var ordemAssociada = nota.Ordens?.FirstOrDefault(o => o.DataCriacao.HasValue);

            TimeSpan? vidaNota = DateTime.UtcNow - nota.DataNota;
            TimeSpan? vidaOrdem = ordemAssociada?.DataCriacao != null && ordemAssociada?.DataCriacao > nota.DataNota
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
            TimeSpan? vidaNota = notaAssociada?.DataNota != null && ordem.DataCriacao < notaAssociada?.DataNota
                ? DateTime.UtcNow - notaAssociada?.DataNota
                : (TimeSpan?)null;

            rankingTotal.Add(((notaAssociada?.IdNota, vidaNota), (ordem.IdOrdem, vidaOrdem)));
        }

        // limpa e ordena gerando um top 20
        var top = rankingTotal
            .Where(r => r.Item1.Item2.HasValue || r.Item2.Item2.HasValue)
            .OrderByDescending(r => r.Item1.Item2 ?? r.Item2.Item2 ?? TimeSpan.MaxValue)
            .Take(7)
            .ToList();

        return top;
    }
}