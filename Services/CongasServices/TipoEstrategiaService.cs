using app_pm_back.Data;
using app_pm_back.Models.CongasModel;

namespace app_pm_back.Services.CongasServices;

public class TipoEstrategiaService : BaseCrudService<TipoEstrategiaModel>
{
    public TipoEstrategiaService(AppDbContext context) : base(context)
    {
        
    }
}