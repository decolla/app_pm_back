using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

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
    /// Duracao planejada em horas.
    /// </summary>
    public decimal? DuracaoHoras { get; set; }

    /// <summary>
    /// Numero de profissionais necessarios.
    /// </summary>
    [Range(1, 1000)]
    public int? NumeroProfissionais { get; set; }

    /// <summary>
    /// Data e hora de inicio.
    /// </summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>
    /// Data e hora de fim.
    /// </summary>
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Tipo de atividade da operacao.
    /// </summary>
    [MaxLength(30)]
    public string? TipoAtividade { get; set; }

    /// <summary>
    /// Texto breve da operacao.
    /// </summary>
    [MaxLength(300)]
    public string? TextoBreve { get; set; }

    /// <summary>
    /// Texto detalhado da operacao.
    /// </summary>
    [MaxLength(2000)]
    public string? TextoDetalhado { get; set; }

    /// <summary>
    /// Ferramentas e kits necessarios para a operação.
    /// </summary>
    public virtual ICollection<FerramentasTipoModel> FerramentasNecessarias { get; set; } = new List<FerramentasTipoModel>();

    /// <summary>
    /// Ordem associada.
    /// </summary>
    [ForeignKey(nameof(OrdemId))]
    public virtual OrdemManutencaoModel Ordem { get; set; } = null!;
}
