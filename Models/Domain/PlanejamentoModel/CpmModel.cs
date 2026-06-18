using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa o modelo de domínio para um Centro de Planejamento e Manutenção (CPM).
/// </summary>
public class CpmModel
{
    /// <summary>
    /// Identificador único do CPM.
    /// </summary>
    [Key]
    public long IdCPM { get; set; }

    /// <summary>
    /// Nome do centro de planejamento.
    /// </summary>
    [MaxLength(150)]
    public required string NomeCentro { get; set; }

    /// <summary>
    /// Organização à qual o CPM pertence.
    /// </summary>
    [MaxLength(100)]
    public required string Organizacao { get; set; }

    /// <summary>
    /// Descrição detalhada do CPM.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Identificador do escopo associado (armazena apenas o ID).
    /// </summary>
    public long IdEscopo { get; set; } // Stores Escopo ID only

    public virtual ICollection<GpmModel> Gpms { get; set; } = new List<GpmModel>();
    public virtual ICollection<CtmModel> Ctms { get; set; } = new List<CtmModel>();
}
