using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class ConfiguracaoGlobalService
{
    private readonly AppDbContext _context;
    
    public ConfiguracaoGlobalService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ConfiguracaoGlobalModel> ObterConfiguracaoAsync()
    {
        // Se não existir (caso não use HasData), cria o primeiro com padrões
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
    
        // Mapeia os Enums diretamente do DTO enviado pelo Front-end
        config.FichasConfiabilidade = dto.FichasConfiabilidade;
        config.ClassificacaoAutomatica = dto.ClassificacaoAutomatica;
        config.CriterioDataRenovacao = dto.CriterioDataRenovacao;
        config.CriterioConfiabilidadeRenovacao = dto.CriterioConfiabilidadeRenovacao;
        config.DataModificacao = DateTime.UtcNow;
        config.ModificadoPorUsuario = usuario;

        await _context.SaveChangesAsync();
    }
}