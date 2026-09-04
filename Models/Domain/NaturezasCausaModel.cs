using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;
public class NaturezasCausaModel
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
    /// <example>Inundação, vandalismo, sujeira, etc</example>
    public string PalavraChave { get; set; }
    
    /// <summary>
    /// Qual o tipo de classificação, só pode ser Corretiva (true) ou Ocorrencia Alheia (False)
    /// </summary>
    public bool EhCorretiva { get; set; }
    
    
    
}
