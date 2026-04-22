using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

public class FerramentasTipoModel
{
    /// <summary>
    /// Id da ferramenta
    /// </summary>
    [Key]
    public int IdFerramenta { get; set; }

    /// <summary>
    /// Checkbox para selecionar a ferramenta
    /// </summary>
    public bool Checkbox { get; set; }
    
    /// <summary>
    /// Nome de identificação da ferramenta
    /// </summary>
    public required string Nome { get; set; }
    
    /// <summary>
    /// Descrição da ferramenta
    /// </summary>
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Quantidade disponível da ferramenta
    /// </summary>
    public int QuantidadeDisp { get; set; }
    
    /// <summary>
    /// Chave estrangeira para TipoManutencao
    /// </summary>
    public int TipoManutencaoId { get; set; }
    [ForeignKey(nameof(TipoManutencaoId))]
    public virtual TipoManutencaoModel TipoManutencao { get; set; } = null!;
}