using app_pm_back.Data;
using app_pm_back.Models.CongasModel;

namespace app_pm_back.Services.CongasServices;

public class TipoManutencaoService : BaseCrudService<TipoManutencaoModel>
{
    public TipoManutencaoService(AppDbContext context) : base(context)
    {
        
    }
    
    
}