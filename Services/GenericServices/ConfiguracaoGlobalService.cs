using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.GenericServices;

public class ConfiguracaoGlobalService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ConfiguracaoGlobalService> _logger;

    public ConfiguracaoGlobalService(AppDbContext context, ILogger<ConfiguracaoGlobalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ConfiguracaoGlobalModel> ObterConfiguracaoAsync()
    {
        // caso não exista, cria uma nova
        var config = await _context.Set<ConfiguracaoGlobalModel>().FirstOrDefaultAsync(c => c.Id == 1);
        if (config == null)
        {
            config = new ConfiguracaoGlobalModel();
            _context.Set<ConfiguracaoGlobalModel>().Add(config);
            await _context.SaveChangesAsync();
        }

        return config;
    }
    
    public async Task<List<string>> ObterNaturezaCausaAsync(bool corretiva)
    {
        var config = await ObterConfiguracaoAsync();

        if (corretiva) return config.PalavrasNaturezaCausaCorretiva;
        return config.PalavrasNaturezaCausaAlheia;
    }

    public async Task SalvarConfiguracaoAsync(ConfiguracaoGlobalDto dto, string usuario)
    {
        var config = await ObterConfiguracaoAsync();

        // mapeia os enums para os valores do banco
        config.FichasConfiabilidade = dto.FichasConfiabilidade;
        config.ClassificacaoAutomatica = dto.ClassificacaoAutomatica;
        config.CriterioDataRenovacao = dto.CriterioDataRenovacao;
        config.CriterioConfiabilidadeRenovacao = dto.CriterioConfiabilidadeRenovacao;
        config.CriarFichasPreventivas = dto.CriarFichasPreventivas;
        config.DataModificacao = DateTime.UtcNow;
        config.ModificadoPorUsuario = usuario;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Configuração global salva com sucesso: {config}", config);
    }

    public async Task SalvarModeloCatboost(string nomeModelo)
    {
        var config = await ObterConfiguracaoAsync();
        
        config.ModeloCatboostPadrao = nomeModelo;
        await _context.SaveChangesAsync();
    }
}