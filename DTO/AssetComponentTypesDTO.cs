using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.Restauracao;

/// <summary>
/// DTO para resposta da API de Ativos da APM.
/// Representa um conjunto de componentes/ativos associados a uma LocalInstalacao (LI).
/// </summary>
public class AssetComponentTypesDTO
{
    /// <summary>
    /// Nome da LocalInstalacao (LI/Name)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Lista de ComponentTypes (IdAtivos) associados a esta LI
    /// </summary>
    public List<ComponentType>? ComponentTypes { get; set; } = null;
}