using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa a entidade de Almoxarifado no sistema.
/// </summary>
public class AlmoxarifadoModel
{
    /// <summary>
    /// Identificador único do Almoxarifado.
    /// </summary>
    [Key]
    public long IdAlmoxarifado { get; set; }

    /// <summary>
    /// Nome do Almoxarifado.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Nome da Organização à qual o almoxarifado pertence.
    /// </summary>
    [MaxLength(100)]
    public required string Organizacao { get; set; }

    /// <summary>
    /// StatusFichas atual do almoxarifado (ex: Ativo, Inativo).
    /// </summary>
    [MaxLength(50)]
    public string? Status { get; set; }

    /// <summary>
    /// Descrição detalhada sobre o almoxarifado.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Identificador do Escopo associado.
    /// </summary>
    public long IdEscopo { get; set; } 

    public virtual ICollection<FerramentaModel> Ferramentas { get; set; } = new List<FerramentaModel>();
    public virtual ICollection<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
}
