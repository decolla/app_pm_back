using System.Net.Http.Headers;
using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Serviço para obter linha temporal de LIs consultando API externa (APM)
/// e cruzando com notas/ordens do banco de dados local com cache
/// </summary>
public class LinhaTemporalService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LinhaTemporalService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _apmBaseUrl;
    
    private const string CacheKeyApmData = "LinhaTemporal_ApmData";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public LinhaTemporalService(
        AppDbContext context,
        ILogger<LinhaTemporalService> logger,
        IConfiguration configuration,
        HttpClient httpClient,
        IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
        _cache = cache;
        _apmBaseUrl = configuration["APIs:APM"] ?? "http://10.3.192.200:60141";
    }

    /// <summary>
    /// Obtém todas as LIs do APM (do cache ou da API)
    /// </summary>
    public async Task<List<LiApmInfo>> ObterTodasLisDoApmAsync()
    {
        if (_cache.TryGetValue(CacheKeyApmData, out List<LiApmInfo>? cached) && cached != null)
        {
            _logger.LogInformation("Dados do APM obtidos do cache ({QtdLIs} LIs)", cached.Count);
            return cached;
        }

        _logger.LogInformation("Cache miss - buscando dados do APM na API externa");
        var dados = await BuscarTodasLisDaApiExternaAsync();
        
        if (dados.Any())
        {
            _cache.Set(CacheKeyApmData, dados, CacheExpiration);
            _logger.LogInformation("Dados do APM armazenados no cache ({QtdLIs} LIs)", dados.Count);
        }

        return dados;
    }

    /// <summary>
    /// Obtém notas e ordens para uma LI específica
    /// </summary>
    public async Task<LinhaTemporalItem> ObterItensDaLiAsync(string idLi)
    {
        var notasTask = BuscarNotasDaLiAsync(idLi);
        var ordensTask = BuscarOrdensDaLiAsync(idLi);

        await Task.WhenAll(notasTask, ordensTask);

        var eventos = new List<EventoLinhaTemporal>();

        foreach (var nota in notasTask.Result)
        {
            eventos.Add(new EventoLinhaTemporal(
                Numero: nota.NumeroNota.ToString(),
                Data: nota.DataNota ?? DateTime.MinValue,
                Tipo: "Nota",
                IdReferencia: nota.IdNota,
                Descricao: nota.DescricaoBreve ?? nota.AnomaliaObservada,
                Classificacao: nota.Classificacao,
                ConfiabilidadeRestaurada: nota.ConfiabilidadeRestaurada?.ToString()
            ));
        }

        foreach (var ordem in ordensTask.Result)
        {
            eventos.Add(new EventoLinhaTemporal
            (
                Numero: ordem.NumeroOrdem?.ToString() ?? ordem.IdOrdem.ToString(),
                Data: ordem.DataCriacao ?? DateTime.MinValue,
                Tipo: "Ordem",
                IdReferencia: ordem.IdOrdem,
                Descricao: ordem.DescricaoBreve,
                Classificacao: ordem.Classificacao,
                ConfiabilidadeRestaurada: ordem.ConfiabilidadeRestaurada?.ToString()
            ));
        }

        return new LinhaTemporalItem
        (
            idLi,
            eventos.OrderByDescending(e => e.Data).ToList()
        );
    }

    /// <summary>
    /// Força atualização do cache
    /// </summary>
    public async Task<int> AtualizarCacheAsync()
    {
        _cache.Remove(CacheKeyApmData);
        var dados = await BuscarTodasLisDaApiExternaAsync();
        if (dados.Any())
            _cache.Set(CacheKeyApmData, dados, CacheExpiration);
        return dados.Count;
    }

    private async Task<List<LiApmInfo>> BuscarTodasLisDaApiExternaAsync()
    {
        try
        {
            var url = $"{_apmBaseUrl}/api/V1/Asset/Installation/Date";
            _logger.LogInformation("Chamando API APM: GET {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<LiApmInfo>>(content, options) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar LIs do APM");
            throw;
        }
    }

    private Task<List<NotaModel>> BuscarNotasDaLiAsync(string idLi) =>
        _context.PM_Notas
            .Where(n => n.IdLi == idLi && n.Classificacao != "NÃO CLASSIFICADA")
            .OrderByDescending(n => n.DataNota)
            .ToListAsync();

    private Task<List<OrdemModel>> BuscarOrdensDaLiAsync(string idLi) =>
        _context.PM_Ordens
            .Where(o => o.IdLi == idLi && o.Classificacao != "NÃO CLASSIFICADA")
            .OrderByDescending(o => o.DataCriacao)
            .ToListAsync();
}

// Classes internas simples (não são DTOs de banco, apenas transport)
public record LiApmInfo(string IdLi, string? DnsIot, DateTime Data);

public record LinhaTemporalItem(string IdLi, List<EventoLinhaTemporal> Eventos);

public record EventoLinhaTemporal(
    string Numero,
    DateTime Data,
    string Tipo,
    long IdReferencia,
    string? Descricao,
    string? Classificacao,
    string? ConfiabilidadeRestaurada
);