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
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// DNS do IoT associado a esta LI (LI/DnsIot)
    /// </summary>
    public string? DnsIot { get; set; }
    
    /// <summary>
    /// Indica se a LI existe ou não na APM. Se false, os campos DnsIot e ComponentTypes podem ser ignorados.
    /// </summary>
    public bool? Exists {  get; set; }

    /// <summary>
    /// Lista de ComponentTypes (IdAtivos) associados a esta LI
    /// </summary>
    public List<ComponentType>? ComponentTypes { get; set; } = null;
}