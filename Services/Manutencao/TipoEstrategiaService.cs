using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe TipoEstrategiaService.
/// </summary>
public class TipoEstrategiaService : BaseCrudService<TipoEstrategiaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoEstrategiaService"/>.
    /// </summary>
    public TipoEstrategiaService(AppDbContext context) : base(context)
    {
    }
}