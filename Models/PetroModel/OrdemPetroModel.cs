using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models;

public class OrdemPetroModel : IClassifiable
{
    /// <summary>
    /// Chave estrangeira que conecta com a NotaPetroModel
    /// </summary>
    [Required]
    public int IdNota { get; set; }

    /// <summary>
    /// Id gerado para a ordem
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdOrdem { get; set; }
        
    /// <summary>
    /// O resultado do tipo da nota
    /// </summary>
    /// <example>CORRETIVA ou PREVENTIVA</example>
    public string? Classificacao { get; set; }
    
    /// <summary>
    /// Quem/o que tomou a decisão da classificação da nota
    /// </summary>
    /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
    public string? MetodoClassificacao { get; set;}
    
    /// <summary>
    /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
    /// </summary>
    public string? ClassificadoPor { get; set; }
    
    /// <summary>
    /// Data de quando a classificacao ocorreu
    /// </summary>
    public DateTime? DataClassificacao { get; set; }

    /// <summary>
    /// Estado atual da ordem 
    /// </summary>
    /// <example>CONCLUIDA</example>
    public string Status { get; set; } = "ABERTA"; 

    /// <summary>
    /// Data de criacao da ordem 
    /// </summary>
    /// <example>DD/MM/AAAA</example>>
    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data prevista para conclusão da ordem 
    /// </summary>
    /// <example>DD/MM/AAAA</example>>
    public DateTime? DataConclusao { get; set; }

    /// <summary>
    /// Observações adicionais a serem adicionadas
    /// </summary>
    public string? ObservacoesEquipe { get; set; }
    
    // Propriedade de navegação do Entity Framework
    // Isso permite acessar nota.Ordem ou ordem.Nota diretamente
    [ForeignKey("IdNota")]
    public virtual NotaPetroModel? NotaOrigem { get; set; }
}