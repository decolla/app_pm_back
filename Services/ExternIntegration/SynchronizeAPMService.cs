using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using app_pm_back.Data;
using app_pm_back.DTO.Restauracao;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Services.ExternIntegationsServices;

/// <summary>
/// Sincroniza os dados de APM com as notas e ordens de serviço do sistema.
/// </summary>
public class SynchronizeAPMService
{
    private readonly AppDbContext _context;
    private readonly ExternIntegrationService _externIntegrationService;
    private readonly ILogger<SynchronizeAPMService> _logger;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SynchronizeAPMService"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="externIntegrationService"></param>
    /// <param name="logger"></param>
    public SynchronizeAPMService(AppDbContext context, ExternIntegrationService externIntegrationService,
        ILogger<SynchronizeAPMService> logger)
    {
        _context = context;
        _externIntegrationService = externIntegrationService;
        _logger = logger;
    }

    /// <summary>
    /// Sincroniza os dados de APM (Asset Performance Management) com as notas e ordens de serviço do sistema.
    /// </summary>
    /// <returns> Bool se a operação ocorreu corretamente</returns>
    public virtual async Task<bool> SynchronizeApmAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando a sincronização dos dados do APM com as Notas e Ordens.");

        // Define um timeout de 30 segundos para a requisição da API e do banco
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        try
        {
            _logger.LogInformation("Chamando API APM: GET /api/V1/Asset/Components/All com timeout de 30s.");

            var jsonResponse = await _externIntegrationService
                .GetDataAsync("/api/V1/Asset/Components/All", cts.Token);

            _logger.LogInformation("Resposta da API APM recebida com sucesso. Tamanho: {Length} caracteres.",
                jsonResponse?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning("Resposta vazia retornada pela API APM.");
                return false;
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ativos = JsonSerializer.Deserialize<List<AssetComponentTypesDto>>(jsonResponse, jsonOptions);

            if (ativos == null || !ativos.Any())
            {
                _logger.LogWarning("Nenhum ativo desserializado ou lista vazia obtida do APM.");
                return false;
            }

            _logger.LogInformation("Desserializados {Count} ativos do APM.", ativos.Count);

            // gera a lista de ativos válidos
            var ativosValidos = ativos
                .Where(a => !string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.DnsIot) &&
                            a.ComponentTypes != null)
                .DistinctBy(a => a.Name)
                .ToList();

            _logger.LogInformation(
                "Ativos válidos gerados com {Count} registros únicos após filtragem e remoção de duplicados.",
                ativosValidos.Count);

            if (!ativosValidos.Any())
            {
                _logger.LogWarning("Nenhum ativo com Name e DnsIot válidos.");
                return false;
            }

            // Mapeamentos rápidos O(1) para busca por Name e por sapId
            var dnsByName = ativosValidos.ToDictionary(a => a.Name!, a => (DnsIot: a.DnsIot, ComponentTypes: a.ComponentTypes), StringComparer.OrdinalIgnoreCase);
            var dnsBySap = ativosValidos.Where(a => !string.IsNullOrEmpty(a.sapId))
                .DistinctBy(a => a.sapId)
                .ToDictionary(a => a.sapId!, a => (DnsIot: a.DnsIot, ComponentTypes: a.ComponentTypes), StringComparer.OrdinalIgnoreCase);

            // extrai chaves separadas para queries eficientes
            var nomesValidos = dnsByName.Keys.ToList();
            var sapsValidos = dnsBySap.Keys.ToList();

            // busca notas e ordens que os LI batem com os passados
            _logger.LogInformation("Buscando Notas e Ordens correspondentes no banco de dados.");

            var notasEncontradas = await _context.PM_Notas
                .Where(n => n.IdLi != null && (nomesValidos.Contains(n.IdLi) || sapsValidos.Contains(n.IdLi)))
                .ToListAsync(cts.Token);

            var ordensEncontradas = await _context.PM_Ordens
                .Where(o => o.IdLi != null && (nomesValidos.Contains(o.IdLi) || sapsValidos.Contains(o.IdLi)))
                .ToListAsync(cts.Token);

            _logger.LogInformation(
                "Encontradas {NotasCount} notas e {OrdensCount} ordens no banco de dados para atualização.",
                notasEncontradas.Count, ordensEncontradas.Count);

            int notasAtualizadas = 0;
            int ordensAtualizadas = 0;

            // atualiza cada uma delas com busca direta O(1)
            foreach (var nota in notasEncontradas)
            {
                if (nota.IdLi != null && (dnsByName.TryGetValue(nota.IdLi, out var duple) || dnsBySap.TryGetValue(nota.IdLi, out duple)))
                {
                    if (nota.IdsAtivo != null && nota.IdsAtivo.RemoveAll(a => duple.ComponentTypes?.Contains(a) != true) > 0)
                        notasAtualizadas++;

                    if (nota.DnsIot != duple.DnsIot)
                    {
                        nota.DnsIot = duple.DnsIot;
                        notasAtualizadas++;
                    }
                }
            }

            foreach (var ordem in ordensEncontradas)
            {
                if (ordem.IdLi != null && (dnsByName.TryGetValue(ordem.IdLi, out var duple) || dnsBySap.TryGetValue(ordem.IdLi, out duple)))
                {
                    if (ordem.IdsAtivo != null && ordem.IdsAtivo.RemoveAll(a => duple.ComponentTypes?.Contains(a) != true) > 0)
                    {
                        ordensAtualizadas++;
                    }

                    if (ordem.DnsIot != duple.DnsIot)
                    {
                        ordem.DnsIot = duple.DnsIot;
                        ordensAtualizadas++;
                    }
                }
            }

            _logger.LogInformation(
                "Campos atualizados em memória: {NotasAtualizadas} notas e {OrdensAtualizadas} ordens.",
                notasAtualizadas, ordensAtualizadas);

            cancellationToken.ThrowIfCancellationRequested();

            if (notasAtualizadas > 0 || ordensAtualizadas > 0)
            {
                _logger.LogInformation("Salvando alterações no banco de dados.");
                var alteracoes = await _context.SaveChangesAsync(cts.Token);
                _logger.LogInformation("Sincronização concluída com sucesso. Linhas afetadas no banco: {Alteracoes}",
                    alteracoes);
                return alteracoes > 0;
            }

            _logger.LogInformation("Sincronização concluída. Nenhuma alteração de DnsIot foi necessária.");
            return true;
        }
        catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
        {
            _logger.LogError(ex, "A sincronização com a API do APM falhou devido a timeout (30 segundos).");
            throw new TimeoutException("A sincronização com o APM expirou (timeout de 30 segundos).", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante a sincronização dos dados do APM.");
            throw;
        }
    }
}