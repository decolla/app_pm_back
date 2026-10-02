using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para o Gerenciamento de Planejamento Mensal (GPM).
/// Exemplo: IdGPM = 1, Nome = "Planejamento Q1"
/// </summary>
public class GpmModel
{
    /// <summary> Identificador único do GPM. </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdGPM { get; set; }

    /// <summary> Nome descritivo do GPM. Exemplo: "GPM Operacional" </summary>
    [MaxLength(150)]
    public string? Nome { get; set; }

    /// <summary> Detalhes adicionais sobre o planejamento. </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary> Chave estrangeira para o CPM relacionado. </summary>
    public long? IdCPM { get; set; }

    /// <summary>
    /// Propriedade de navegação para o modelo CPM.
    /// </summary>
    [ForeignKey(nameof(IdCPM))]
    public virtual CpmModel? CPM { get; set; }

    /// <summary>
    /// Coleção de usuários vinculados a este GPM.
    /// </summary>
    public virtual ICollection<UsuarioModel>? Usuarios { get; set; } = new List<UsuarioModel>();
}
