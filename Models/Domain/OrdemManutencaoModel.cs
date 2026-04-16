using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Ordem de manutencao para execucao de servico planejado ou corretivo.
/// </summary>
public class OrdemManutencaoModel
{
    /// <summary>
    /// Identificador da ordem.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdOrdem { get; set; }

    /// <summary>
    /// Tipo da ordem (ex.: ZM01, ZM02, ZM03).
    /// </summary>
    [MaxLength(10)]
    public required string TipoOrdem { get; set; }

    /// <summary>
    /// Status atual da ordem.
    /// </summary>
    [MaxLength(20)]
    public string? StatusOrdem { get; set; }

    /// <summary>
    /// Nota relacionada.
    /// </summary>
    public long? NotaRelacionadaId { get; set; }

    /// <summary>
    /// Plano de manutencao associado (usado em preventivas).
    /// </summary>
    [MaxLength(40)]
    public string? PlanoManutencaoCodigo { get; set; }

    /// <summary>
    /// Prioridade da ordem.
    /// </summary>
    [Range(1, 4)]
    public int? Prioridade { get; set; }

    /// <summary>
    /// Data de entrada da ordem.
    /// </summary>
    public DateTime? DataEntrada { get; set; }

    /// <summary>
    /// Usuario criador da ordem.
    /// </summary>
    public int? CriadaPorUsuarioId { get; set; }

    /// <summary>
    /// Usuario que modificou a ordem pela ultima vez.
    /// </summary>
    public int? ModificadaPorUsuarioId { get; set; }

    /// <summary>
    /// Custo estimado.
    /// </summary>
    public decimal? CustoEstimado { get; set; }

    /// <summary>
    /// Custo planejado.
    /// </summary>
    public decimal? CustoPlanejado { get; set; }

    /// <summary>
    /// Custo real.
    /// </summary>
    public decimal? CustoReal { get; set; }

    /// <summary>
    /// Nota relacionada.
    /// </summary>
    [ForeignKey(nameof(NotaRelacionadaId))]
    public virtual NotaManutencaoModel? NotaRelacionada { get; set; }

    /// <summary>
    /// Plano de manutencao relacionado.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoCodigo))]
    public virtual PlanoManutencaoModel? PlanoManutencao { get; set; }

    /// <summary>
    /// Usuario criador da ordem.
    /// </summary>
    [ForeignKey(nameof(CriadaPorUsuarioId))]
    public virtual UsuarioSapModel? CriadaPorUsuario { get; set; }

    /// <summary>
    /// Usuario que alterou a ordem.
    /// </summary>
    [ForeignKey(nameof(ModificadaPorUsuarioId))]
    public virtual UsuarioSapModel? ModificadaPorUsuario { get; set; }

    /// <summary>
    /// Operacoes planejadas ou executadas na ordem.
    /// </summary>
    public virtual ICollection<OperacaoOrdemModel> Operacoes { get; set; } = new List<OperacaoOrdemModel>();
}

