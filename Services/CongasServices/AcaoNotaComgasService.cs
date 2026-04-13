using app_pm_back.Data;
using app_pm_back.Models.CongasModel;

namespace app_pm_back.Services.CongasServices;

public class AcaoNotaComgasService : BaseCrudService<AcaoNotaCongasModel>
{
    public AcaoNotaComgasService(AppDbContext context) : base(context)
    {
    }
    
    public async Task CreateAcoesEmMassaAsync(List<AcaoNotaCongasModel> acoes)
    {
        if (acoes.Any())
        {
            await _context.AcaoNotaCongas.AddRangeAsync(acoes);
            await _context.SaveChangesAsync();
        }
    }
}