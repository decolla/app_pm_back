using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Fichas.Dashboard;

public class HistogramaDashboard
{
    public HistogramaDashboard()
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

    public List<object> TempoMedioGrupo(List<OrdemModel> ordens, List<NotaModel> notas)
    {
        var dadosOrdens = ordens
            .Where(o => o.DataCriacao.HasValue && o.FimReal.HasValue && o.GPM != null)
            .Select(o => new
            {
                Grupo = o.GPM,
                DataReferencia = o.DataCriacao.Value,
                DiasTratamento = (o.FimReal.Value - o.DataCriacao.Value).TotalDays
            });
        
        var dadosNotas = notas
            .Where(n => n.DataNota.HasValue && n.GPM != null)
            .Select(n => new
            {
                Nota = n,
                OrdemConcluida = n.Ordens.FirstOrDefault(o => o.FimReal.HasValue)
            })
            .Where(x => x.OrdemConcluida != null)
            .Select(x => new
            {
                Grupo = x.Nota.GPM,
                DataReferencia = x.Nota.DataNota.Value,
                DiasTratamento = (x.OrdemConcluida.FimReal.Value - x.Nota.DataNota.Value).TotalDays
            });

        // combina os dados de ordens e notas em uma única lista
        var todosOsDados = dadosOrdens.Concat(dadosNotas).ToList();

        if (!todosOsDados.Any()) return new List<object>();

        // alinha a contagem a cada 7 dias
        var dataMinimaGlobal = todosOsDados.Min(d => d.DataReferencia).Date;

        // agrupa por data
        var resultadoHistograma = todosOsDados
            .GroupBy(d => new
            {
                d.Grupo,
                // calcula o número do intervalo de 7 dias desde a data mínima global
                NumeroIntervalo = (int)(d.DataReferencia.Date - dataMinimaGlobal).TotalDays / 7
            })
            .Select(g =>
            {
                var dataInicioIntervalo = dataMinimaGlobal.AddDays(g.Key.NumeroIntervalo * 7);
                var dataFimIntervalo = dataInicioIntervalo.AddDays(6);
                var tempoMedioEmDias = g.Average(d => d.DiasTratamento);

                return new
                {
                    GrupoTrab = g.Key.Grupo,
                    IntervaloSeteDias = $"{dataInicioIntervalo:dd/MM/yyyy} a {dataFimIntervalo:dd/MM/yyyy}",
                    TotalFichasNoPeriodo = g.Count(),
                    TempoMedioDias = Math.Round(tempoMedioEmDias, 2),
                    TempoMedioFormatado = TimeSpan.FromDays(tempoMedioEmDias).ToString(@"dd\.hh\:mm\:ss")
                };
            })
            .OrderBy(r => r.GrupoTrab)
            .ThenBy(r => r.IntervaloSeteDias)
            .ToList<object>();

        return resultadoHistograma;
    }

    public List<Object> HistogramaGrupo(List<OrdemModel> ordens, List<NotaModel> notas)
    {
        var dadosOrdens = ordens
            .Where (o => o.GPM != null && o.StatusOrdem != null)
            .Select(o => new
            {
                Grupo = o.GPM,
                Status = o.StatusOrdem
            });
        
        var dadosNotas = notas
            .Where(n => n.GPM != null && n.StatusNota != null)
            .Select(n => new
            {
                Grupo = n.GPM,
                Status = n.StatusNota
            });
        
        // combina os dados de ordens e notas em uma única lista
        var todosOsDados = dadosOrdens.Concat(dadosNotas).ToList();

        if (!todosOsDados.Any()) return new List<object>();

        // agrupa por grupo e calcula a média de dias de tratamento
        var resultadoHistograma = todosOsDados;

        return null;
    }
}