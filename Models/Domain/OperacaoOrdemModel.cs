using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Operacao da ordem de manutencao.
/// </summary>
public class OperacaoOrdemModel
{
    /// <summary>
    /// Identificador da operacao.
    /// </summary>
    [Key]
    public int IdOperacao { get; set; }

    /// <summary>
    /// Ordem dona da operacao.
    /// </summary>
    public long OrdemId { get; set; }

    /// <summary>
    /// Nome da operacao.
    /// </summary>
    [MaxLength(120)]
    public required string NomeOperacao { get; set; }

    /// <summary>
    /// Tipo de atividade da operacao.
    /// </summary>
    [MaxLength(30)]
    public string? TipoAtividade { get; set; }

    /// <summary>
    /// Duracao planejada em horas.
    /// </summary>
    public decimal? DuracaoHoras { get; set; }

    /// <summary>
    /// Numero de profissionais necessarios.
    /// </summary>
    public int? NumeroProfissionais { get; set; }

    /// <summary>
    /// Texto breve da operacao.
    /// </summary>
    [MaxLength(300)]
    public string? TextoBreve { get; set; }

    /// <summary>
    /// Data e hora de inicio.
    /// </summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>
    /// Data e hora de fim.
    /// </summary>
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Ordem associada.
    /// </summary>
    [ForeignKey(nameof(OrdemId))]
    public virtual OrdemManutencaoModel Ordem { get; set; } = null!;
}

