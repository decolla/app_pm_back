using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe ImobilizadoService.
/// </summary>
public class ImobilizadoService : BaseCrudService<ImobilizadoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ImobilizadoService"/>.
    /// </summary>
    public ImobilizadoService(AppDbContext context) : base(context)
    {
    }
}