using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class ImobilizadoService : BaseCrudService<ImobilizadoModel>
{
    public ImobilizadoService(AppDbContext context) : base(context)
    {
    }
}
