using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

public class ItemRenovacaoTipoModel
{
    /// <summary>
    /// Id do item de renovacao.
    /// </summary>
    [Key]
    public int IdItemRenovacaoTipo { get; set; }

    /// <summary>
    /// Codigo do item de renovacao.
    /// </summary>
    public required string CodigoItem { get; set; }
    
    /// <summary>
    /// Observações do item de renovacao.
    /// </summary>
    public string? Observacoes { get; set; }
    
    /// <summary>
    /// Template1 do item de renovacao.
    /// </summary>
    public string? Template1 { get; set; }
    
    /// <summary>
    /// Template2 do item de renovacao.
    /// </summary>
    public string? Template2 { get; set; }
    
    /// <summary>
    /// FK do tipo de manutencao.
    /// </summary>
    public int TipoManutencaoId { get; set; }
    [ForeignKey(nameof(TipoManutencaoId))]
    public virtual TipoManutencaoModel TipoManutencao { get; set; } = null!;
}