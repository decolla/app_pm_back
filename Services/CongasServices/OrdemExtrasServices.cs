using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class AnaliseRiscoService : BaseCrudService<AnaliseRiscoModel>
{
    public AnaliseRiscoService(AppDbContext context) : base(context)
    {
    }
}

public class AnaliseRiscoOperacionalService : BaseCrudService<AnaliseRiscoOperacionalModel>
{
    public AnaliseRiscoOperacionalService(AppDbContext context) : base(context)
    {
    }
}

public class PermissaoTrabalhoService : BaseCrudService<PermissaoTrabalhoModel>
{
    public PermissaoTrabalhoService(AppDbContext context) : base(context)
    {
    }
}

public class MatrizLibraService : BaseCrudService<MatrizLibraModel>
{
    public MatrizLibraService(AppDbContext context) : base(context)
    {
    }
}

public class OperacaoOrdemService : BaseCrudService<OperacaoOrdemModel>
{
    public OperacaoOrdemService(AppDbContext context) : base(context)
    {
    }
}

