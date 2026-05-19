using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.CongasServices;

public class UsuarioSapService : BaseCrudService<UsuarioSapModel>
{
    public UsuarioSapService(AppDbContext context) : base(context)
    {
    }
}

public class FabricanteService : BaseCrudService<FabricanteModel>
{
    public FabricanteService(AppDbContext context) : base(context)
    {
    }
}

public class EnderecoService : BaseCrudService<EnderecoModel>
{
    public EnderecoService(AppDbContext context) : base(context)
    {
    }
}

public class LocalizacaoService : BaseCrudService<LocalizacaoModel>
{
    public LocalizacaoService(AppDbContext context) : base(context)
    {
    }
}

public class CentroPlanejamentoManutencaoService : BaseCrudService<CentroPlanejamentoManutencaoModel>
{
    public CentroPlanejamentoManutencaoService(AppDbContext context) : base(context)
    {
    }
}

public class CentroTrabalhoService : BaseCrudService<CentroTrabalhoModel>
{
    public CentroTrabalhoService(AppDbContext context) : base(context)
    {
    }
}

public class GrupoPlanejamentoManutencaoService : BaseCrudService<GrupoPlanejamentoManutencaoModel>
{
    public GrupoPlanejamentoManutencaoService(AppDbContext context) : base(context)
    {
    }
}

