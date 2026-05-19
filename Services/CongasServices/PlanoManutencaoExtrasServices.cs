using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class CicloManutencaoService : BaseCrudService<CicloManutencaoModel>
{
    public CicloManutencaoService(AppDbContext context) : base(context)
    {
    }
}

public class ItemPlanoManutencaoService : BaseCrudService<ItemPlanoManutencaoModel>
{
    public ItemPlanoManutencaoService(AppDbContext context) : base(context)
    {
    }
}

