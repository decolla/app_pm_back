using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class ImobilizadoService : BaseCrudService<ImobilizadoModel>
{
    public ImobilizadoService(AppDbContext context) : base(context)
    {
    }

    public async Task<ImobilizadoModel?> GetByTagAsync(string tagImobilizado)
    {
        return await _context.PM_Imobilizados.FindAsync(tagImobilizado);
    }

    public async Task<bool> DeleteByTagAsync(string tagImobilizado)
    {
        var entity = await _context.PM_Imobilizados.FindAsync(tagImobilizado);
        if (entity == null)
        {
            return false;
        }

        _context.PM_Imobilizados.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}

public class LocalInstalacaoService : BaseCrudService<LocalInstalacaoModel>
{
    public LocalInstalacaoService(AppDbContext context) : base(context)
    {
    }

    public async Task<LocalInstalacaoModel?> GetByCodigoAsync(string codigoLocalInstalacao)
    {
        return await _context.PM_LocaisInstalacao.FindAsync(codigoLocalInstalacao);
    }

    public async Task<bool> DeleteByCodigoAsync(string codigoLocalInstalacao)
    {
        var entity = await _context.PM_LocaisInstalacao.FindAsync(codigoLocalInstalacao);
        if (entity == null)
        {
            return false;
        }

        _context.PM_LocaisInstalacao.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}

public class ComponenteEquipamentoService : BaseCrudService<ComponenteEquipamentoModel>
{
    public ComponenteEquipamentoService(AppDbContext context) : base(context)
    {
    }
}

