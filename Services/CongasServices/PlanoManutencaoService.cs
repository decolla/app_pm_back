using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class PlanoManutencaoService : BaseCrudService<PlanoManutencaoModel>
{
    private readonly DbSet<NotaManutencaoModel> _notaManutencaoModels;
    private readonly DbSet<LocalInstalacaoModel> _localInstalacaoModels;
    private readonly DbSet<ItemPlanoManutencaoModel> _itemPlanoManutencaoModels;
    private readonly DbSet<PlanoManutencaoModel> _maintancePlanComgasModels;
    
    public PlanoManutencaoService(AppDbContext context) : base(context)
    {
        _notaManutencaoModels = context.Set<NotaManutencaoModel>();
        _maintancePlanComgasModels = context.Set<PlanoManutencaoModel>();
        _localInstalacaoModels = context.Set<LocalInstalacaoModel>();
        _itemPlanoManutencaoModels = context.Set<ItemPlanoManutencaoModel>();
    }
    
    public async Task CreatePlanosEmMassaAsync(List<PlanoManutencaoModel> planos)
    {
        if (planos.Any())
        {
            await _context.PlanosManutencaoNormalizados.AddRangeAsync(planos);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<object?> AtivoGerald(string ativo)
    {
        // seleciona todas os LI associadas ao ativo
        List<LocalInstalacaoModel> locais = await _localInstalacaoModels
            .Where(n => n.CodigoLocalInstalacao == ativo)
            .ToListAsync();
        
        // seleciona todos os itens de manutenção associados aos locais
        List<ItemPlanoManutencaoModel> itens = await _itemPlanoManutencaoModels
            .Where(i => locais.Any(l => l.CodigoLocalInstalacao == i.LocalInstalacaoCodigo))
            .ToListAsync();

        // encontra os planos de manutenção associados aos itens
        List<PlanoManutencaoModel> planosManut = await _maintancePlanComgasModels
            .Where(p => itens.Any(i => i.PlanoManutencaoCodigo == p.CodigoPlano))
            .ToListAsync(); //alterar dps para busca melhorada, sem puxar todos os planos
        
        // encontra o último plano criado 
        PlanoManutencaoModel? ultimoPlano = planosManut
            .OrderByDescending(p => p.DataCriacao)
            .FirstOrDefault();

        if(ultimoPlano == null) return null;

        return new
        {
            DataCriacao = ultimoPlano.DataCriacao,
            Estrategia = ultimoPlano.Estrategia,
            Ciclo = ultimoPlano.Estrategia.Ciclo,
            Plano = ultimoPlano
        };
    }
}