using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Nota de manutencao contendo sintomas, causa e plano de execucao.
/// </summary>
public class NotaManutencaoModel
{
    /// <summary>
    /// Identificador da nota.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdNota { get; set; }

    /// <summary>
    /// Tipo de nota (ex.: MG, M9, VT, OR).
    /// </summary>
    [MaxLength(10)]
    public required string TipoNota { get; set; }

    /// <summary>
    /// Descricao breve da nota.
    /// </summary>
    [MaxLength(250)]
    public required string Descricao { get; set; }

    /// <summary>
    /// Descricao detalhada da nota.
    /// </summary>
    [MaxLength(4000)]
    public string? DescricaoDetalhada { get; set; }

    /// <summary>
    /// Status atual da nota.
    /// </summary>
    [MaxLength(20)]
    public string? StatusNota { get; set; }

    /// <summary>
    /// Data de abertura da nota.
    /// </summary>
    public DateTime DataNota { get; set; }

    /// <summary>
    /// Prioridade de 1 a 4 (4 = baixa).
    /// </summary>
    [Range(1, 4)]
    public int Prioridade { get; set; }

    /// <summary>
    /// Inicio desejado para execucao.
    /// </summary>
    public DateTime? InicioDesejado { get; set; }

    /// <summary>
    /// Conclusao desejada para a nota.
    /// </summary>
    public DateTime? ConclusaoDesejada { get; set; }

    /// <summary>
    /// TAG do equipamento relacionado.
    /// </summary>
    [MaxLength(60)]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Codigo do local de instalacao relacionado.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }

    /// <summary>
    /// Usuario que criou a nota.
    /// </summary>
    public int? CriadoPorUsuarioId { get; set; }

    /// <summary>
    /// Equipamento associado a nota.
    /// </summary>
    [ForeignKey(nameof(EquipamentoTag))]
    public virtual EquipamentoAtivoModel? Equipamento { get; set; }

    /// <summary>
    /// Local de instalacao associado a nota.
    /// </summary>
    [ForeignKey(nameof(LocalInstalacaoCodigo))]
    public virtual LocalInstalacaoModel? LocalInstalacao { get; set; }

    /// <summary>
    /// Usuario criador da nota.
    /// </summary>
    [ForeignKey(nameof(CriadoPorUsuarioId))]
    public virtual UsuarioSapModel? CriadoPorUsuario { get; set; }

    /// <summary>
    /// Itens de diagnostico da nota.
    /// </summary>
    public virtual ICollection<ItemNotaManutencaoModel> Itens { get; set; } = new List<ItemNotaManutencaoModel>();

    /// <summary>
    /// Medidas sugeridas para a nota.
    /// </summary>
    public virtual ICollection<MedidaNotaModel> Medidas { get; set; } = new List<MedidaNotaModel>();

    /// <summary>
    /// Atividades sugeridas para a nota.
    /// </summary>
    public virtual ICollection<AtividadeNotaModel> Atividades { get; set; } = new List<AtividadeNotaModel>();

    /// <summary>
    /// Ordens relacionadas a nota.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> Ordens { get; set; } = new List<OrdemManutencaoModel>();
}

