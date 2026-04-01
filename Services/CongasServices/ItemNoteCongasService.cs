using app_pm_back.Data;
using app_pm_back.Models.CongasModel;

namespace app_pm_back.Services.CongasServices;

public class ItemNoteCongasService : BaseCrudService<ItemNotaCongasModel>
{
    public ItemNoteCongasService(AppDbContext context) : base(context)
    {
        
    }
}