using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class FuncionarioService : BaseCrudService<FuncionarioModel>
{
    public FuncionarioService(AppDbContext context) : base(context)
    {
    }
}

public class SetorVendasService : BaseCrudService<SetorVendasModel>
{
    public SetorVendasService(AppDbContext context) : base(context)
    {
    }
}

public class AlmoxarifadoService : BaseCrudService<AlmoxarifadoModel>
{
    public AlmoxarifadoService(AppDbContext context) : base(context)
    {
    }
}

