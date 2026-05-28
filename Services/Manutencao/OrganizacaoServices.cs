using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services.Manutencao;
/// <summary>
/// Classe AlmoxarifadoService.
/// </summary>

public class AlmoxarifadoService : BaseCrudService<AlmoxarifadoModel>
{
 /// <summary>
 /// Inicializa uma nova instância da classe <see cref="AlmoxarifadoService"/>.
 /// </summary>
 public AlmoxarifadoService(AppDbContext context) : base(context)
 {
 }
}
