using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Services.FichasServices;

/// <summary>
/// Serviço para obter linha temporal de LIs consultando API externa (APM)
/// com paginação direta na API e cruzando com notas/ordens do banco de dados local
/// </summary>

public class ApmResponse
{
    public List<LiApmInfo> Assets { get; set; } = new();
    public int Total { get; set; }
}
public class LinhaTemporalService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LinhaTemporalService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apmBaseUrl;
    

    public LinhaTemporalService(
        AppDbContext context,
        ILogger<LinhaTemporalService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _context = context;
        _logger = logger;
        _httpClient = httpClient;
        _apmBaseUrl = Config.RouteConfig.GetTwinsBaseUrl(configuration);
    }

    /// <summary>
    /// Obtém LIs do APM com paginação direta na API externa
    /// </summary>
    /// <param name="pagina">Número da página (base 1)</param>
    /// <param name="tamanho">Tamanho da página</param>
    /// <returns>Lista de LIs da página solicitada</returns>
    public async Task<ApmResponse> ObterLisDoApmAsync(int pagina, int tamanho)
    {
        try
        {
            // Assumindo que a API do APM suporta paginação via query parameters
            // Ajuste os nomes dos parâmetros conforme a documentação da API do APM
            pagina = pagina - 1;
            if (pagina < 0)
            {
                pagina = 0;
            }
            var url = $"{_apmBaseUrl}/api/V1/Asset/InstallationDate?pageNumber={pagina}&pageSize={tamanho}";
            _logger.LogInformation("Chamando API APM: GET {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _logger.LogInformation("Recebidos os LI: {content} e options: {options}. Com paginas {Pagina} e tamanho {Tamanho}", 
                content, options, pagina, tamanho);
            return JsonSerializer.Deserialize<ApmResponse>(content, options) ?? new ApmResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar LIs do APM (página {Pagina}, tamanho {Tamanho})", pagina, tamanho);
            throw;
        }
    }

    /// <summary>
    /// Obtém notas e ordens para uma LI específica
    /// </summary>
    public async Task<LinhaTemporalItem> ObterItensDaLiAsync(string idLi)
    {
        var notasTask = await BuscarNotasDaLiAsync(idLi);
        var ordensTask =  await BuscarOrdensDaLiAsync(idLi);

        var eventos = new List<EventoLinhaTemporal>();

        foreach (var nota in notasTask)
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

        foreach (var ordem in ordensTask)
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

    private async Task<List<NotaModel>> BuscarNotasDaLiAsync(string idLi)
    {
        //Busca o registro na tabela Reliability usando o IdLi
        var reliability = await _context.PM_Reliability
            .FirstOrDefaultAsync(r => r.IdLi == idLi);

        //erifica se o registro existe e se a lista de IDs não está nula ou vazia
        if (reliability?.IdsNotas == null || !reliability.IdsNotas.Any())
        {
            return new List<NotaModel>(); // Retorna vazio sem bater na tabela de Notas
        }

        //Busca as Notas usando o .Contains() com a lista resgatada
        return await _context.PM_Notas
            .Where(n => reliability.IdsNotas.Contains(n.IdNota))
            .OrderByDescending(n => n.DataNota)
            .ToListAsync();
    }

    private async Task<List<OrdemModel>> BuscarOrdensDaLiAsync(string idLi)
    {
        //Busca o registro na tabela Reliability usando o IdLi
        var reliability = await _context.PM_Reliability
            .FirstOrDefaultAsync(r => r.IdLi == idLi);

        //Verifica se o registro existe e se a lista de IDs não está nula/vazia
        if (reliability?.IdsOrdens == null || !reliability.IdsOrdens.Any())
        {
            return new List<OrdemModel>(); // Retorna vazio sem bater na tabela de Ordens
        }

        //Busca as Ordens usando o .Contains() com a lista resgatada
        return await _context.PM_Ordens
            .Where(o => reliability.IdsOrdens.Contains(o.IdOrdem))
            .OrderByDescending(o => o.DataCriacao)
            .ToListAsync();
    }
}

// Classes internas simples (não são DTOs de banco, apenas transport)
public record LiApmInfo(
    [property: JsonPropertyName("sapName")] string IdLi,
    [property: JsonPropertyName("dnsIot")] string? DnsIot,
    [property: JsonPropertyName("installationDate")] DateTime? Data
);

public record LinhaTemporalItem(string IdLi, List<EventoLinhaTemporal> Eventos);

public record EventoLinhaTemporal(
    string Numero,
    DateTime Data,
    string Tipo,
    long IdReferencia,
    string? Descricao,
    Classificacao Classificacao,
    string? ConfiabilidadeRestaurada
);