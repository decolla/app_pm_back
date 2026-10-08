using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Fichas.Dashboard;

public class HistogramaDashboard
{

    public HistogramaDashboard(DashboardService dashboardService)
    {
    }

    /// <summary>
    /// Gera a relação de status de ordem e relação de prazo de notas
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="unidadeTempo"></param>
    /// <returns></returns>
    public (List<object>, List<object>) RelacaoStatusOrdensSemana(List<NotaModel> notas, UnidadeTempo unidadeTempo)
    {
        var notasComData = notas.Where(n => n.DataNota.HasValue).ToList();
        if (!notasComData.Any()) return (new(), new());

        List<object> relacaoStatus = new();
        List<object> relacaoPrazo = new();

        var notasOrdenadas = notasComData.OrderBy(n => n.DataNota).ToList();
        DateTime dataInicial = notasOrdenadas.First().DataNota!.Value;

        int valorTemporal = 1;

        if (unidadeTempo == UnidadeTempo.Meses)
            valorTemporal = 30;
        else if (unidadeTempo == UnidadeTempo.Anos)
            valorTemporal = 365;

        // agrupa as notas por semana
        var notasPorSemana = notasOrdenadas
            .GroupBy(n => (int)((n.DataNota!.Value - dataInicial).TotalDays / valorTemporal) + 1)
            .ToDictionary(g => g.Key, g => g.ToList());

        int totalSemanas = (int)((DateTime.Now - dataInicial).TotalDays / valorTemporal) + 1;

        for (int tempo = 1; tempo <= totalSemanas; tempo++)
        {
            if (!notasPorSemana.TryGetValue(tempo, out var notasTempo) || notasTempo.Count == 0)
            {
                // // se a série for muito longa
                // if (totalSemanas > 52)
                //     continue;

                notasTempo = new List<NotaModel>();
            }

            int notasComOrdem = notasTempo.Select(n => n.Ordens).Count(o => o.Count != 0);
            int notasSemOrdem = notasTempo.Count - notasComOrdem;
            int notasRejeitadas = notasTempo.Count(n => n.StatusNota == StatusFichas.Reprovada);
            int notasConcluidas = notasTempo.Count(n => n.StatusNota == StatusFichas.Concluida);

            relacaoStatus.Add(new
            {
                Indice = tempo,
                NotasSemOrdem = notasSemOrdem,
                NotasComOrdem = notasComOrdem,
                NotasRejeitadas = notasRejeitadas,
                NotasConcluidas = notasConcluidas
            });

            int notasComOrdemPrazo = notasTempo.Count(n => n.Ordens.Any(o => o.FimPlanejado > DateTime.Now));
            int notasSemOrdemPrazo = notasTempo.Count(n =>
                n.Ordens.Count == 0 && n.DataAvaria.HasValue && n.FimPlanejado < DateTime.Now);
            int notasSemOrdemAtrasadas = notasTempo.Count(n => n.Ordens.Any(o => o.FimReal < DateTime.Now));
            int notasComOrdemAtrasadas = notasTempo.Count(n => n.Ordens.Any(o => o.FimReal > DateTime.Now));

            relacaoPrazo.Add(new
            {
                Indice = tempo,
                NotasSemOrdemPrazo = notasSemOrdemPrazo,
                NotasComOrdemPrazo = notasComOrdemPrazo,
                NotasSemOrdemAtrasadas = notasSemOrdemAtrasadas,
                NotasComOrdemAtrasadas = notasComOrdemAtrasadas
            });
        }
        

        return (relacaoStatus, relacaoPrazo);
    }
}