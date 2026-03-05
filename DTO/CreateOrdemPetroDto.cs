using System.ComponentModel.DataAnnotations;

namespace app_pm_back.DTO;

public class CreateOrdemPetroDto
{
    /// <summary>
    /// Tipo de ordem gerada: Corretiva ou Preventiva
    /// </summary>
    /// <example>Preventiva</example>
    [Required]
    public string TipoOrdem { get; set; } 
    
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
}