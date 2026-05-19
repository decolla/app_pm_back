using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class MedidaNotaService : BaseCrudService<MedidaNotaModel>
{
    public MedidaNotaService(AppDbContext context) : base(context)
    {
    }
}

public class AtividadeNotaService : BaseCrudService<AtividadeNotaModel>
{
    public AtividadeNotaService(AppDbContext context) : base(context)
    {
    }
}

