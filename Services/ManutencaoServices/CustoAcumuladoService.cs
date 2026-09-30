using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Services.GenericServices;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

public class CustoAcumuladoService
{
    private readonly AppDbContext _context;
    GenericSearchService _genericSearchService;

    public CustoAcumuladoService(GenericSearchService genericSearchService)
    {
        _genericSearchService = genericSearchService;
    }

    private decimal SimularCustoFicha(long idFicha)
    {
        decimal fatorAleatorio = idFicha % 10;
        return 100m + (fatorAleatorio * 90.50m);
    }

    public async Task<List<PontoCurvaCustoDto>> CalcularCustoAcumulado(string idLi, DateTime dataInicio,
        DateTime dataFim)
    {
        var notasBanco = await _context.PM_Notas.AsNoTracking()
            .Where(n => n.IdLi == idLi && n.DataNota >= dataInicio && n.DataFim <= dataFim && n.DataNota != null)
            .Select(n => new { n.IdNota, Data = n.DataNota!.Value })
            .ToListAsync();

        var notas = notasBanco.Select(n => (
            Data: n.Data,
            Custo: SimularCustoFicha(n.IdNota),
            Tipo: "Real"
        )).ToList();

        var ordensBanco = await _context.PM_Ordens.AsNoTracking()
            .Where(o => o.IdLi == idLi && o.InicioReal >= dataInicio && o.FimReal <= dataFim && o.InicioReal != null)
            .Select(o => new { o.IdOrdem, Data = o.InicioReal!.Value })
            .ToListAsync();
        
        var ordens = ordensBanco.Select(o => (
            Data: o.Data,
            Custo: SimularCustoFicha(o.IdOrdem) * 1.5m,
            Tipo: "Real"
        )).ToList();

        var planos = await _context.PM_Planos.AsNoTracking()
            .Where(p =>p.IdLi == idLi && p.DataInicios <= dataFim && p.DataConclusao >= dataInicio)
            .Include(p => p.TiposPlano)
                .ThenInclude(tp => tp.TipoManutencao)
                    .ThenInclude(tm => tm.Operacoes)
                        .ThenInclude(op => op.Ferramentas)
            .ToListAsync();

        var previsoes = new List<(DateTime Data, decimal Custo, string Tipo)>();

        foreach (var plano in planos)
        {
            if (plano.DataInicios == null) continue;

            foreach (var tipo in plano.TiposPlano)
            {
                if(tipo.Dias == null || tipo.Dias <= 0) continue;

                decimal custoBase = 0;
                var operacoes = tipo.TipoManutencao?.Operacoes;
                
                if (operacoes != null)
                {
                    foreach (var op in operacoes)
                    {
                        custoBase += (decimal)(op.CustosAdicionais ?? 0); 
                        if (op.Ferramentas != null)
                        {
                            custoBase += op.Ferramentas.Sum(f => (decimal)(f.ValorCompra ?? 0));
                        }
                    }
                }
                
                if (custoBase == 0) continue;

                DateTime dataProjetada = plano.DataInicios.Value.AddDays(tipo.Dias.Value);
                
                if (tipo.Continua == true)
                {
                    while (dataProjetada <= dataFim)
                    {
                        if (dataProjetada >= dataInicio) previsoes.Add(( dataProjetada, custoBase, "Previsto" ));
                        dataProjetada = dataProjetada.AddDays(tipo.Dias.Value);
                    }
                }
                else
                {
                    if (dataProjetada >= dataInicio && dataProjetada <= dataFim)
                        previsoes.Add(( dataProjetada, custoBase, "Previsto" ));
                }
            }
        }
        
        var todosEventos = notas.Concat(ordens).Concat(previsoes)
            .OrderBy(e => e.Data)
            .ToList();

        var resultado = new List<PontoCurvaCustoDto>();
        decimal acumuladoReal = 0;
        decimal acumuladoPrevisto = 0;

        foreach (var evt in todosEventos)
        {
            if (evt.Tipo == "Real") acumuladoReal += evt.Custo;
            if (evt.Tipo == "Previsto") acumuladoPrevisto += evt.Custo;
            
            resultado.Add(new PontoCurvaCustoDto
            {
                Data = evt.Data,
                CustoRealAcumulado = acumuladoReal,
                CustoPrevistoAcumulado = acumuladoPrevisto
            });
        }

        return resultado;
    }
}