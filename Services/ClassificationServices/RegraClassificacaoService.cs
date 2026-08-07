using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.FichasServices;

public class RegraClassificacaoService : BaseCrudService<RegraClassificacaoModel>
{
    public RegraClassificacaoService(AppDbContext context) : base(context)
    {
    }
}