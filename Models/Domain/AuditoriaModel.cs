using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Models.Domain;

public class AuditoriaModel
{
    /// <summary>
    /// Identificador da auditoria
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdAuditoria { get; set; }

    /// <summary>
    /// O Id de quem fez a alteração.
    /// </summary>
    [MaxLength(200)]
    public string? IdUsuario { get; set; } 
    
    /// <summary>
    /// Nome da tabela que foi alterada.
    /// </summary>
    [MaxLength(200)]
    public string NomeTabela { get; set; } = string.Empty;

    /// <summary>
    /// Ação realizada na tabela.
    /// </summary>
    [MaxLength(30)]
    public EntityState Acao { get; set; }
    
    /// <summary>
    /// O valor da chave primária do registro alterado.
    /// </summary>
    public string ChavePrimaria { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON com os valores antigos e novos.
    /// </summary>
    public string? ValoresAntigos { get; set; }
    
    /// <summary>
    /// JSON com os valores novos.
    /// </summary>
    public string? ValoresNovos { get; set; } 
    
    /// <summary>
    /// Data que ocorreu tal alteração.
    /// </summary>
    public DateTime DataAlteracao { get; set; } = DateTime.UtcNow;
}