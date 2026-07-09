using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para CTM (Centro de Trabalho Móvel).
/// </summary>
public class CtmModel
{
    /// <summary>
    /// Identificador único do GPM associado ao CTM.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdCTM { get; set; }
    
    /// <summary>
    /// Nome do CTM.
    /// </summary>
    [MaxLength(100)]
    public required string Nome { get; set; }

    /// <summary>
    /// Nome ou identificação da área do CTM.
    /// </summary>
    [MaxLength(100)]
    public required string AreaCTM { get; set; }

    /// <summary>
    /// Descrição detalhada do CTM.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Chave estrangeira para o modelo CPM relacionado.
    /// </summary>
    public long IdCPM { get; set; }

    /// <summary>
    /// Propriedade de navegação para o modelo CPM.
    /// </summary>
    [ForeignKey(nameof(IdCPM))]
    public virtual CpmModel? CPM { get; set; }

    public virtual ICollection<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
}
