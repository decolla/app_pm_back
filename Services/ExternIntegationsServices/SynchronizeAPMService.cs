using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.DTO.Restauracao;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.ExternIntegationsServices;

/// <summary>
/// Sincroniza os dados de APM com as notas e ordens de serviço do sistema.
/// </summary>
public class SynchronizeAPMService
{
    private readonly AppDbContext _context;
    private readonly ExternIntegrationService _externIntegrationService;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SynchronizeAPMService"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="externIntegrationService"></param>
    public SynchronizeAPMService(AppDbContext context, ExternIntegrationService externIntegrationService)
    {
        _context = context;
        _externIntegrationService = externIntegrationService;
    }

    /// <summary>
    /// Sincroniza os dados de APM (Asset Performance Management) com as notas e ordens de serviço do sistema.
    /// </summary>
    /// <returns>bool se a operação ocorreu corretamente</returns>
    public async Task<bool> SynchronizeAPMAsync()
    {
        // realiza o GET no endereço
        var jsonResponse = await _externIntegrationService
            .GetDataAsync("/api/V1/Asset/Components/All");

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var ativos = JsonSerializer.Deserialize<List<AssetComponentTypesDTO>>(jsonResponse, jsonOptions);

        if (ativos == null || !ativos.Any()) return false;

        // gera um dicionário com nome e dnsIot
        Dictionary<string,string?> dicionarioDns = ativos
            .Where(a => !string.IsNullOrEmpty(a.Name) && !string.IsNullOrEmpty(a.DnsIot))
            .DistinctBy(a => a.Name)
            .ToDictionary(a => a.Name, a => a.DnsIot);
        
        if (!dicionarioDns.Any()) return false;
        
        var nomesLi = dicionarioDns.Keys.ToList();

        // busca notas e ordens que os LI batem com os passados
        var notasEncontradas = await _context.PM_Notas
            .Where(n => n.IdLi != null && nomesLi.Contains(n.IdLi))
            .ToListAsync();

        var ordensEncontradas = await _context.PM_Ordens
            .Where(n => n.IdLi != null && nomesLi.Contains(n.IdLi))
            .ToListAsync();

        // atualiza cada uma delas 
        foreach (var nota in notasEncontradas)
        {
            if (dicionarioDns.TryGetValue(nota.IdLi!, out var dnsIot))
            {
                nota.DnsIot = dnsIot;
            }
        }
        
        foreach (var ordem in ordensEncontradas)
        {
            if (dicionarioDns.TryGetValue(ordem.IdLi!, out var dnsIotExterno))
            {
                ordem.DnsIot = dnsIotExterno;
            }
        }
        
        var alteracoes = await _context.SaveChangesAsync();
        
        return alteracoes > 0;
    }
}