using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class PlanoManutencaoService : BaseCrudService<PlanoModel>
{
    public PlanoManutencaoService(AppDbContext context) : base(context)
    {
    }
    
    public async Task CreatePlanosEmMassaAsync(List<PlanoModel> planos)
    {
        if (planos.Any())
        {
            await _context.PM_Planos.AddRangeAsync(planos);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PlanoModel> CreateComTiposAsync(PlanoModel novo)
    {
        var criado = await CreateAsync(novo);

        var estrategia = await _context.PM_Estrategias
            .FirstOrDefaultAsync(e => e.IdEstrategia == criado.IdEstrategia);

        if (estrategia == null)
        {
            throw new Exception("Estrategia informada nao encontrada.");
        }

        // Get TipoEstrategias for this strategy
        var tiposEstrategias = await _context.PM_TiposEstrategia
            .Where(te => te.IdEstrategia == estrategia.IdEstrategia)
            .ToListAsync();

        var tiposPlanos = tiposEstrategias.Select(te => new TipoPlanoModel
        {
            IdPlano = criado.IdPlano,
            IdTipoEstrategia = te.IdTipoEstrategia,
            Dias = te.Dias,
            ToleranciaPositiva = te.ToleranciaPositiva,
            ToleranciaNegativa = te.ToleranciaNegativa,
            Continua = te.Continua,
            Categoria = te.Categoria,
            IdTipoCep = te.IdTipoCep,
            IdTipoManutencao = te.IdTipoManutencao,
            Visivel = true
        }).ToList();

        await _context.PM_TiposPlano.AddRangeAsync(tiposPlanos);
        await _context.SaveChangesAsync();

        return criado;
    }
}