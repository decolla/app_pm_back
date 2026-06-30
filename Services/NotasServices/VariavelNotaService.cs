using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe VariavelNotaService.
/// </summary>
public class VariavelNotaService : BaseCrudService<VariavelNotaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="VariavelNotaService"/>.
    /// </summary>
    public VariavelNotaService(AppDbContext context) : base(context)
    {
    }
}
