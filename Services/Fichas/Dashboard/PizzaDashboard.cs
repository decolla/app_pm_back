using app_pm_back.Models.Domain;

namespace app_pm_back.Services.FichasServices;

public class PizzaDashboard
{
    public PizzaDashboard()
    {
    }

    /// <summary>
    /// Estatísticas de status de notas 
    /// </summary>
    /// <param name="notas"></param>
    /// <param name="totalNotas"></param>
    /// <returns></returns>
    public object RelacaoStatusOrdens(List<NotaModel> notas, int totalNotas)
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
    public object NotasAbertasRelacaoPrazo(List<NotaModel> notas)
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
    /// Estatísticas de notas do usuário atual
    /// </summary>
    /// <returns></returns>
    public Dictionary<StatusFichas, int> NotasUsuario(List<NotaModel> notas)
    {
        
        if (!notas.Any())
        {
            return new Dictionary<StatusFichas, int>();
        }
        
        Dictionary<StatusFichas, int> pedidos = new Dictionary<StatusFichas, int>();

        // incrementa o contador para cada status de nota
        foreach (var nota in notas)
        {
            StatusFichas? status = nota.StatusNota;

            if (status == null)
            {
                pedidos[StatusFichas.Aberta]++;
            }
            else
            {
                pedidos[status.Value]++;
            }
        }

        return pedidos;
    }
}