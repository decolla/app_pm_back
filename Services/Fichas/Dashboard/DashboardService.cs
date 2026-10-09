using app_pm_back.Data;
using app_pm_back.Dto.Fichas;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using app_pm_back.Services.FichasServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace app_pm_back.Services.Fichas.Dashboard;

/// <summary>
/// Gera as estatísticas do dashboard 
/// </summary>
public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;
    private readonly IMemoryCache _cache;
    private readonly IUserContext _userContext;
    private readonly PizzaDashboard _pizzaDashboard;
    private readonly RankingDashboard _rankingDashboard;
    private readonly HistogramaDashboard _histogramaDashboard;

    public DashboardService(AppDbContext context, ILogger<DashboardService> logger, IMemoryCache cache, IUserContext userContext)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
        _userContext = userContext;
        _pizzaDashboard = new PizzaDashboard();
        _rankingDashboard = new RankingDashboard();
        _histogramaDashboard = new HistogramaDashboard();
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
            
            var usuario = _userContext.GetUsuarioIdAtual();
            if (usuario == null)
                _logger.LogError("Não foi possível obter o ID do usuário atual.");
            
            // aplica a filtragem em notas e ordens
            List<NotaModel> notas = await GetNotasAsync();
            List<NotaModel> notasUser = notas
                .Where(n => n.Usuarios.Any(u => u.IdUsuario == usuario))
                .ToList();

            List<OrdemModel> ordens = await GetOrdensAsync();

            var statusSemana = _histogramaDashboard.RelacaoStatusOrdensSemana(notas, UnidadeTempo.Meses);

            if(dashboardDto == null)
                dashboardDto = new DashboardDto();
            
            dashboardDto.RelacaoStatusOrdens = _pizzaDashboard.RelacaoStatusOrdens(notas, notas.Count);
            dashboardDto.NotasAbertasRelacaoPrazo = _pizzaDashboard.NotasAbertasRelacaoPrazo(notas);
            dashboardDto.TopLiOcorrencias = _rankingDashboard.TopLiOcorrencias(notas, ordens)
                .ToDictionary(x => x.Key, x => (x.Value.notas, x.Value.ordens));
            dashboardDto.RelacaoStatusOrdensSemana = statusSemana.Item1;
            dashboardDto.NotasAbertasRelacaoPrazoSemana = statusSemana.Item2;
            dashboardDto.RankingTempoVida = _rankingDashboard.RankingTempoVida(notas, ordens)
                .Select(x =>
                (
                    (x.Item1.Item1, x.Item1.Item2),
                    (x.Item2.Item1, x.Item2.Item2)
                ))
                .ToList();
            dashboardDto.NotasUsuario = _pizzaDashboard.NotasUsuario(notasUser);
            dashboardDto.TempoMedioGrupo = _histogramaDashboard.TempoMedioGrupo(ordens, notas);
            dashboardDto.FichasGrupo = _histogramaDashboard.HistogramaGrupo(ordens, notas);
            
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
}