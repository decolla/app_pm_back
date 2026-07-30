using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

public class RegraClassificacaoDto
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
    /// Qual o tipo de classificação aplicada pela regra 
    /// </summary>
    /// <example>0,1,2</example>
    public Classificacao ClassificacaoAplicada { get; set; }

    /// <summary>
    /// Se a regra é para uma palavra permitida ou proibida 
    /// </summary>
    public bool Permitida { get; set; } = true;
    
    /// <summary>
    /// Em caso de conflito, qual a prioridade da regra acima das outras
    /// </summary>
    /// <example>1</example>
    public int OrdemPrioridade { get; set; }
}