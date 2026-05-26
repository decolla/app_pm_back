using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class AlmoxarifadoService : BaseCrudService<AlmoxarifadoModel>
{
    public AlmoxarifadoService(AppDbContext context) : base(context)
    {
    }
}
