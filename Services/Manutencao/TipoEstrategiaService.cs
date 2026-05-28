using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;

public class TipoEstrategiaService : BaseCrudService<TipoEstrategiaModel>
{
 public TipoEstrategiaService(AppDbContext context) : base(context)
 {
 }
}