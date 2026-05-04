using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models;

/// <summary>
/// PM_Regras para classificação de notas/ordens
/// </summary>
public class RuleClassificationModel
{
    /// <summary>
    /// Id da Regra
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdRegra { get; set; }

    /// <summary>
    /// Palavra chave usada para a filtragem 
    /// </summary>
    /// <example>"Vazamento", "Inspecionar", "Limpeza"</example>
    public string PalavraChave { get; set; }
    
    /// <summary>
    /// Campo alvo onde as palavras da regra serão aplicadas
    /// </summary>
    /// <example>"Descricao", "TextoBreve"</example>
    public string CampoAlvo { get; set; }
    
    /// <summary>
    /// Qual o tipo de classificação aplicada pela regra 
    /// </summary>
    /// <example>"CORRETIVA", "PREVENTIVA", "DÚBIAS", "OUTRAS"</example>
    public string ClassificacaoAplicada { get; set; }
    
    /// <summary>
    /// Em caso de conflito, qual a prioridade da regra acima das outras
    /// </summary>
    /// <example>1</example>
    public int OrdemPrioridade { get; set; }
}