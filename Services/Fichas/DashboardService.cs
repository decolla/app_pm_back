using System.Runtime.InteropServices.JavaScript;
using app_pm_back.Data;
using app_pm_back.DTO.Buscas;
using app_pm_back.Dto.Fichas;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace app_pm_back.Services.FichasServices;

/// <summary>
/// Gera as estatísticas do dashboard 
/// </summary>
public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;
    private readonly IMemoryCache _cache;
    private readonly IUserContext _userContext;

    public DashboardService(AppDbContext context, ILogger<DashboardService> logger, IMemoryCache cache, IUserContext userContext)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// Função principal que chamará todas as outras funções para gerar as estatísticas com baKse nas fichas após filtragem.
    /// </summary>
    /// <param name="filtro">Filtro para notas e ordens</param>
    /// <returns></returns>
    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        long? escopoId = _userContext.GetEscoposDoUsuario().FirstOrDefault();
        var cacheKey = "dashboard_data";
        
        if(escopoId != null)
            cacheKey += $"_escopo_{escopoId}";

        // busca no cache, se não estiver lá, gera os dados
        if (!_cache.TryGetValue(cacheKey, out DashboardDto? dashboardDto))
        {
            _logger.LogInformation("Dados não encontrados no cache. Gerando dados do dashboard...");
            
            // aplica a filtragem em notas e ordens
            List<NotaModel> notas = await GetNotasAsync();
            List<OrdemModel> ordens = await GetOrdensAsync();

            var statusSemana = RelacaoStatusOrdensSemana(notas, UnidadeTempo.Meses);

            if(dashboardDto == null)
                dashboardDto = new DashboardDto();
            
            dashboardDto.RelacaoStatusOrdens = RelacaoStatusOrdens(notas, notas.Count);
            dashboardDto.NotasAbertasRelacaoPrazo = NotasAbertasRelacaoPrazo(notas);
            dashboardDto.TopLiOcorrencias = TopLiOcorrencias(notas, ordens)
                .ToDictionary(x => x.Key, x => (x.Value.notas, x.Value.ordens));
            dashboardDto.RelacaoStatusOrdensSemana = statusSemana.Item1;
            dashboardDto.NotasAbertasRelacaoPrazoSemana = statusSemana.Item2;
            dashboardDto.RankingTempoVida = RankingTempoVida(notas, ordens)
                .Select(x =>
                (
                    (x.Item1.Item1, x.Item1.Item2),
                    (x.Item2.Item1, x.Item2.Item2)
                ))
                .ToList();
            dashboardDto.NotasUsuario = await NotasUsuario();
            
            
            // adiciona ao cache o dashboard gerado, com prioridade alta e expiração de 24 horas
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.High)
                .SetAbsoluteExpiration(TimeSpan.FromHours(24))
                .SetSlidingExpiration(TimeSpan.FromHours(4));;

            _cache.Set(cacheKey, dashboardDto, cacheOptions);
        }

        if (dashboardDto != null) return dashboardDto;
        throw new InvalidOperationException("Não foi possível obter os dados do dashboard.");
    }

    /// <summary>
    /// Aplica filtragem nas notas 
    /// </summary>
    /// <param name="filtro">Filtro estabelecido</param>
    /// <returns>Notas filtradas</returns>
    private async Task<List<NotaModel>> GetNotasAsync()
    {
        try
        {
            var query = _context.PM_Notas
                .Include(n => n.Ordens)
                .AsNoTracking()
                .AsQueryable();

            // retorna notas filtradas
            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Erro ao obter notas:", ex);
        }
    }

    /// <summary>
    /// Filtra ordens 
    /// </summary>
    /// <param name="filtro">Filtro a ser aplicado</param>
    /// <returns>Ordens filtradas</returns>
    private async Task<List<OrdemModel>> GetOrdensAsync()
    {
        try
        {
            var query = _context.PM_Ordens
                .Include(o => o.Nota)
                .ThenInclude(n => n.Ordens)
                .AsQueryable();

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Erro ao obter ordens:", ex);
        }
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

        _logger.LogInformation("Notas abertas com realção ao prazo geradas com suceeso.");

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

        _logger.LogInformation("Top (7) ocorrências de locais de insalação gerados com sucesso.");

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
    /// <param name="unidadeTempo"></param>
    /// <returns></returns>
    private (List<object>, List<object>) RelacaoStatusOrdensSemana(List<NotaModel> notas, UnidadeTempo unidadeTempo)
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
                n.Ordens.Count == 0 && n.DataAvaria.HasValue && n.FimDesejado < DateTime.Now);
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

        _logger.LogInformation("Relação semanal de status e prazos gerados com sucesso.");

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

        _logger.LogInformation("Ranking de top (7) tempos de vida gerado com sucesso.");

        return top;
    }
    
    /// <summary>
    /// Estatísticas de notas do usuário atual
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<StatusFichas, int>> NotasUsuario()
    {
        // busca o usuário 
        var usuario = _userContext.GetUsuarioIdAtual();
        if (usuario == null)
            _logger.LogError("Não foi possível obter o ID do usuário atual.");
        
        var notas = await _context.PM_Usuarios
            .Where(u => u.IdUsuario == usuario)
            .SelectMany(u => u.Notas)
            .ToListAsync();
        
        if (!notas.Any())
        {
            _logger.LogError("Não foi possível obter as notas do usuário.");
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