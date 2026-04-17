using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class EstrategiaComgasService : BaseCrudService<EstrategiaManutencaoModel>
{
    public EstrategiaComgasService(AppDbContext context) : base(context)
    {
        
    }
    
}