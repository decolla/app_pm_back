using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class ConfiguracaoGlobalService
{
    private readonly AppDbContext _context;
    private readonly ClassificationService<NotaModel> _classificationNotaService;
    private readonly ClassificationService<OrdemModel> _classificationOrdemService;


    public ConfiguracaoGlobalService(AppDbContext context, ClassificationService<NotaModel> classificationNotaService,
        ClassificationService<OrdemModel> classificationOrdemService)
    {
        _classificationNotaService = classificationNotaService;
        _classificationOrdemService = classificationOrdemService;
        _context = context;
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

    public async Task SalvarConfiguracaoAsync(ConfiguracaoGlobalDto dto, string usuario)
    {
        var config = await ObterConfiguracaoAsync();

        // mapeia os enums para os valores do banco
        config.FichasConfiabilidade = dto.FichasConfiabilidade;
        config.ClassificacaoAutomatica = dto.ClassificacaoAutomatica;
        config.CriterioDataRenovacao = dto.CriterioDataRenovacao;
        config.CriterioConfiabilidadeRenovacao = dto.CriterioConfiabilidadeRenovacao;
        config.DataModificacao = DateTime.UtcNow;
        config.ModificadoPorUsuario = usuario;
        
        await _context.SaveChangesAsync();
        
        await _classificationNotaService.AutoClassify();
        await _classificationOrdemService.AutoClassify();
    }
}