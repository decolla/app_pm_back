using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe CpmService.
/// </summary>
public class CpmService : BaseCrudService<CpmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CpmService"/>.
    /// </summary>
    public CpmService(AppDbContext context) : base(context)
    {
    }
}

/// <summary>
/// Classe CtmService.
/// </summary>
public class CtmService : BaseCrudService<CtmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CtmService"/>.
    /// </summary>
    public CtmService(AppDbContext context) : base(context)
    {
    }
}

/// <summary>
/// Classe GpmService.
/// </summary>
public class GpmService : BaseCrudService<GpmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="GpmService"/>.
    /// </summary>
    public GpmService(AppDbContext context) : base(context)
    {
    }
}