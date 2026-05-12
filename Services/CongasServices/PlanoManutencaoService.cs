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
            await _context.PM_PlanosManutencaoNormalizados.AddRangeAsync(planos);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PlanoManutencaoModel> CreateComTiposAsync(PlanoManutencaoModel novo)
    {
        var criado = await CreateAsync(novo);

        if (!criado.EstrategiaId.HasValue)
        {
            return criado;
        }

        var estrategia = await _context.Set<EstrategiaManutencaoModel>()
            .Include(e => e.Itens)
            .FirstOrDefaultAsync(e => e.IdEstrategia == criado.EstrategiaId.Value);

        if (estrategia == null)
        {
            throw new Exception("Estrategia informada nao encontrada.");
        }

        if (!estrategia.Itens.Any())
        {
            return criado;
        }

        var tiposPlanos = estrategia.Itens.Select(item => new TipoPlanoModel
        {
            PlanoManutencaoId = criado.CodigoPlano,
            TipoEstrategiaId = item.IdTipoEstrategia,
            Dias = item.Dias,
            ToleranciaPositiva = item.ToleranciaPositiva,
            ToleranciaNegativa = item.ToleranciaNegativa,
            Continua = item.Continua,
            Categoria = item.Categoria,
            TipoManutencaoCepId = item.IdTipoManutencaoCep,
            TipoManutencaoId = item.IdTipoManutencao
        }).ToList();

        await _context.Set<TipoPlanoModel>().AddRangeAsync(tiposPlanos);
        await _context.SaveChangesAsync();

        return criado;
    }
}