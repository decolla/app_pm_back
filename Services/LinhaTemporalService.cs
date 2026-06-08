using System.Net.Http.Headers;
using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.DTO.Restauracao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Serviço para obter linha temporal de LIs consultando API externa
/// e cruzando com notas/ordens do banco de dados local
/// </summary>
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
        _apmBaseUrl = configuration["APIs:APM"] ?? "http://10.3.192.200:60141";
    }

    /// <summary>
    /// Obtém a linha temporal completa para um conjunto de LIs
    /// </summary>
    /// <param name="idsLi">Lista de IDs das Localizações de Instalação (LIs)</param>
    /// <returns>Lista de itens da linha temporal</returns>
    public async Task<List<LinhaTemporalDTO>> ObterLinhaTemporalAsync(List<string> idsLi)
    {
        _logger.LogInformation("Iniciando ObterLinhaTemporalAsync para {QtdLIs} LIs", idsLi?.Count ?? 0);

        if (idsLi == null || idsLi.Count == 0)
        {
            _logger.LogWarning("Lista de LIs vazia ou nula");
            return new List<LinhaTemporalDTO>();
        }

        try
        {
            // 1. Chama API externa para obter (IdLi, DnsIot, Data)
            var lisExternas = await ObterLisDaApiExternaAsync(idsLi);
            
            if (!lisExternas.Any())
            {
                _logger.LogWarning("Nenhuma LI retornada pela API externa");
                return new List<LinhaTemporalDTO>();
            }

            _logger.LogInformation("Recebidas {QtdLIs} LIs da API externa", lisExternas.Count);

            // 2. Para cada LI, busca notas e ordens no banco local
            var resultado = new List<LinhaTemporalDTO>();

            foreach (var liExterna in lisExternas)
            {
                var item = new LinhaTemporalDTO
                {
                    IdLi = liExterna.IdLi,
                    DnsIot = liExterna.DnsIot,
                    Data = liExterna.Data,
                    Eventos = new List<EventoLinhaTemporalDTO>()
                };

                // Busca NOTAS relacionadas a esta LI (por IdLi)
                var notas = await BuscarNotasDaLiAsync(liExterna.IdLi);
                
                foreach (var nota in notas)
                {
                    item.Eventos.Add(new EventoLinhaTemporalDTO
                    {
                        Numero = nota.NumeroNota.ToString(),
                        Data = nota.DataNota ?? DateTime.MinValue,
                        Tipo = "Nota",
                        IdReferencia = nota.IdNota,
                        Descricao = nota.DescricaoBreve ?? nota.AnomaliaObservada,
                        Classificacao = nota.Classificacao,
                        ConfiabilidadeRestaurada = nota.ConfiabilidadeRestaurada?.ToString()
                    });
                }

                // Busca ORDENS relacionadas a esta LI (por IdLi)
                var ordens = await BuscarOrdensDaLiAsync(liExterna.IdLi);
                
                foreach (var ordem in ordens)
                {
                    item.Eventos.Add(new EventoLinhaTemporalDTO
                    {
                        Numero = ordem.NumeroOrdem?.ToString() ?? ordem.IdOrdem.ToString(),
                        Data = ordem.DataCriacao ?? DateTime.MinValue,
                        Tipo = "Ordem",
                        IdReferencia = ordem.IdOrdem,
                        Descricao = ordem.DescricaoBreve,
                        Classificacao = ordem.Classificacao,
                        ConfiabilidadeRestaurada = ordem.ConfiabilidadeRestaurada?.ToString()
                    });
                }

                // Ordena eventos por data (mais recente primeiro)
                item.Eventos = item.Eventos
                    .OrderByDescending(e => e.Data)
                    .ToList();

                resultado.Add(item);
            }

            _logger.LogInformation("Linha temporal gerada com {QtdItens} itens", resultado.Count);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter linha temporal");
            throw;
        }
    }

    /// <summary>
    /// Chama a API externa (APM) para obter lista de LIs com IdLi, DnsIot e Data
    /// </summary>
    private async Task<List<LiExternaDTO>> ObterLisDaApiExternaAsync(List<string> idsLi)
    {
        try
        {
            var url = $"{_apmBaseUrl}/api/V1/Asset/Timeline"; // Ajustar endpoint conforme API real
            
            _logger.LogInformation("Chamando API externa: POST {Url} com {QtdLIs} LIs", url, idsLi.Count);

            var jsonContent = JsonSerializer.Serialize(idsLi);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao chamar API externa. StatusCode: {StatusCode}", response.StatusCode);
                response.EnsureSuccessStatusCode();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Resposta da API externa: {Response}", responseContent);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultado = JsonSerializer.Deserialize<List<LiExternaDTO>>(responseContent, options) 
                          ?? new List<LiExternaDTO>();

            _logger.LogInformation("Obtidas {QtdLIs} LIs da API externa", resultado.Count);
            return resultado;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro na requisição HTTP para API externa");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao desserializar resposta da API externa");
            throw;
        }
    }

    /// <summary>
    /// Busca notas associadas a uma LI (por IdLi)
    /// </summary>
    private async Task<List<NotaModel>> BuscarNotasDaLiAsync(string idLi)
    {
        return await _context.PM_Notas
            .Where(n => n.IdLi == idLi)
            .Where(n => n.Classificacao != "NÃO CLASSIFICADA")
            .OrderByDescending(n => n.DataNota)
            .ToListAsync();
    }

    /// <summary>
    /// Busca ordens associadas a uma LI (por IdLi)
    /// </summary>
    private async Task<List<OrdemModel>> BuscarOrdensDaLiAsync(string idLi)
    {
        return await _context.PM_Ordens
            .Where(o => o.IdLi == idLi)
            .Where(o => o.Classificacao != "NÃO CLASSIFICADA")
            .OrderByDescending(o => o.DataCriacao)
            .ToListAsync();
    }
}

// DTO interno do serviço
internal class LiExternaDTO
{
    public string IdLi { get; set; } = string.Empty;
    public string? DnsIot { get; set; }
    public DateTime Data { get; set; }
}