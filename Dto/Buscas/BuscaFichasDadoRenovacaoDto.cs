using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.Buscas;

/// <summary>
/// DTO para busca de fichas por dados de renovação
/// </summary>
public class BuscaFichasDadoRenovacaoDto
{
    public string? IdLi { get; set; }
    
    public ComponentType? IdAtivo { get; set; }
    
    public OrigemUR? ReferenciaOrigem { get; set; }
}