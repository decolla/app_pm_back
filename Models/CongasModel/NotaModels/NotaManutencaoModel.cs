using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Nota de manutencao contendo sintomas, causa e plano de execucao.
/// </summary>
public class NotaManutencaoModel : IClassifiable
{
    /// <summary>
    /// Identificador da nota.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdNota { get; set; }

    /// <summary>
    /// Número da nota vindo do SAP/Planilha
    /// </summary>
    /// <example>4088673</example>
    [SheetColumn("Nota")]
    public long NumeroNota { get; set; }

    /// <summary>
    /// TAG do equipamento relacionado.
    /// </summary>
    [MaxLength(60)]
    [SheetColumn("Equipamento")]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Data de abertura da nota.
    /// </summary>
    public DateTime DataNota { get; set; }

    /// <summary>
    /// Usuario que criou a nota.
    /// </summary>
    public int? CriadoPorUsuarioId { get; set; }

    /// <summary>
    /// Codigo do local de instalacao relacionado.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }
    
    /// <summary>
    /// Finalidade de criação da nota
    /// </summary>
    [MaxLength(120)]
    public string Finalidade { get; set; }

    /// <summary>
    /// Conclusao desejada para a nota.
    /// </summary>
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Data da avaria associada à abertura da nota.
    /// </summary>
    public DateTime? DataAvaria { get; set; }

    /// <summary>
    /// Hora da avaria associada à abertura da nota.
    /// </summary>
    public TimeSpan? HoraAvaria { get; set; }

    /// <summary>
    /// Indica se o equipamento estava disponível no momento da abertura.
    /// </summary>
    public string? EquipamentoDisponivel { get; set; }

    /// <summary>
    /// Indica se a análise de risco operacional é necessária.
    /// </summary>
    public bool? AnaliseRiscoOperacional { get; set; }

    /// <summary>
    /// Descrição da anomalia observada.
    /// </summary>
    [MaxLength(1000)]
    public string? AnomaliaObservada { get; set; }

    /// <summary>
    /// Observações complementares sobre a nota.
    /// </summary>
    [MaxLength(2000)]
    public string? ObservacoesComplementares { get; set; }

    /// <summary>
    /// Ponto de referência para apoio à intervenção.
    /// </summary>
    [MaxLength(200)]
    public string? PontoReferencia { get; set; }

    /// <summary>
    /// Matriz LIBRA aplicada à triagem da nota.
    /// </summary>
    [MaxLength(80)]
    public string? MatrizLibra { get; set; }

    /// <summary>
    /// Temperatura registrada no momento da ocorrência.
    /// </summary>
    public decimal? Temperatura { get; set; }

    /// <summary>
    /// Tensão elétrica registrada no momento da ocorrência.
    /// </summary>
    public decimal? Tensao { get; set; }

    /// <summary>
    /// Fluido associado ao ponto observado.
    /// </summary>
    [MaxLength(80)]
    public string? Fluido { get; set; }

    /// <summary>
    /// Pressão registrada no momento da ocorrência.
    /// </summary>
    public decimal? Pressao { get; set; }

    /// <summary>
    /// Número de plaqueta do ativo ou componente.
    /// </summary>
    [MaxLength(80)]
    public string? Plaqueta { get; set; }

    /// <summary>
    /// Descricao breve da nota.
    /// </summary>
    [MaxLength(250)]
    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

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
    /// Prioridade de 1 a 4 (4 = baixa).
    /// </summary>
    [Range(1, 4)]
    public int Prioridade { get; set; }

    /// <summary>
    /// Inicio desejado para execucao.
    /// </summary>
    public DateTime? InicioDesejado { get; set; }


    // ================= CATBOOST ===================


    /// <summary>
    /// Tipo de nota (ex.: MG, M9, VT, OR).
    /// </summary>
    [MaxLength(10)]
    public string? TipoNota { get; set; }

    /// <summary>
    /// O resultado do tipo da nota
    /// </summary>
    /// <example>CORRETIVA ou PREVENTIVA</example>
    public string? Classificacao { get; set; }

    /// <summary>
    /// Quem/o que tomou a decisão da classificação da nota
    /// </summary>
    /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
    public string? MetodoClassificacao { get; set; }

    /// <summary>
    /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
    /// </summary>
    public string? ClassificadoPor { get; set; }

    /// <summary>
    /// Data de quando a classificacao ocorreu
    /// </summary>
    public DateTime? DataClassificacao { get; set; }


    // CATBOOST

    /// <summary>
     /// Tipo de prioridade da nota 
     /// </summary>
     [SheetColumn("Tipo prioridade")]
     public string? TipoPrioridade { get; set; }

    /// <summary>
     /// Tipo de nota 
     /// </summary>
     [SheetColumn("Tipo de nota")]
     public string? TipoDeNota { get; set; }

     /// <summary>
     /// Texto do grupo de codificação da nota
     /// </summary>
     [SheetColumn("Txt.grp.codific")]
     public string? TxtGrpCodific { get; set; }

     /// <summary>
     /// Texto do código de codificação da nota
     /// </summary>
     [SheetColumn("Txt.code codif.")]
     public string? TxtCodeCodif { get; set; }


     /// <summary>
     /// Codificação do item
     /// </summary>
     [SheetColumn("Codificação")]
     public string? Codificacao { get; set; }

     /// <summary>
     /// Consequência do item
     /// </summary>
     [SheetColumn("Conseqüência")]
     public int? Consequencia { get; set; }

     /// <summary>
     /// Grupo de códigos do item
     /// </summary>
     [SheetColumn("Grupo de codes")]
     public string? GrupoDeCodes { get; set; }

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
    /// PM_Anexos da nota.
    /// </summary>
    public virtual ICollection<AnnexModel> Anexos { get; set; } = new List<AnnexModel>();
    
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