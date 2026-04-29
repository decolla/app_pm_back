using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Ordem de manutencao para execucao de servico planejado ou corretivo.
/// </summary>
public class OrdemManutencaoModel : IClassifiable
{
    /// <summary>
    /// Identificador da ordem.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdOrdem { get; set; }

    /// <summary>
    /// Tipo da ordem.
    /// </summary>
    /// <example>ZM01, ZM02, ZM03</example>
    [MaxLength(10)]
    public string? TipoOrdem { get; set; }

    /// <summary>
    /// Codigo do trabalho associado à ordem.
    /// </summary>
    [MaxLength(40)]
    public string? CodigoTrabalho { get; set; }

    /// <summary>
    /// Descricao principal da ordem.
    /// </summary>
    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Situação geral da ordem.
    /// </summary>
    [MaxLength(40)]
    public string? Situacao { get; set; }

    /// <summary>
    /// Situação no planejamento da ordem.
    /// </summary>
    [MaxLength(40)]
    public string? SituacaoPlanejamento { get; set; }

    /// <summary>
    /// O resultado do tipo da nota
    /// </summary>
    /// <example>CORRETIVA ou PREVENTIVA</example>
    public string? Classificacao { get; set; }
    
    /// <summary>
    /// Quem/o que tomou a decisão da classificação da nota
    /// </summary>
    /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
    public string? MetodoClassificacao { get; set;}
    
    /// <summary>
    /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
    /// </summary>
    public string? ClassificadoPor { get; set; }
    
    /// <summary>
    /// Data de quando a classificacao ocorreu
    /// </summary>
    public DateTime? DataClassificacao { get; set; }
    
    /// <summary>
    /// Status atual da ordem.
    /// </summary>
    [MaxLength(20)]
    public string? StatusOrdem { get; set; }

    /// <summary>
    /// Inicio planejado da ordem.
    /// </summary>
    public DateTime? InicioPlanejado { get; set; }

    /// <summary>
    /// Fim planejado da ordem.
    /// </summary>
    public DateTime? FimPlanejado { get; set; }

    /// <summary>
    /// Inicio real da ordem.
    /// </summary>
    public DateTime? InicioReal { get; set; }

    /// <summary>
    /// Fim real da ordem.
    /// </summary>
    public DateTime? FimReal { get; set; }

    /// <summary>
    /// TAG do equipamento associado.
    /// </summary>
    [MaxLength(60)]
    public string? EquipamentoTag { get; set; }

    /// <summary>
    /// Codigo do local de instalacao associado.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }

    /// <summary>
    /// Material referenciado na ordem.
    /// </summary>
    [MaxLength(80)]
    public string? Material { get; set; }

    /// <summary>
    /// Numero de serie do item ou dispositivo.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Dados adicionais do dispositivo.
    /// </summary>
    [MaxLength(1000)]
    public string? DadosAdicionaisDispositivo { get; set; }

    /// <summary>
    /// Periodo planejado para execução.
    /// </summary>
    [MaxLength(80)]
    public string? Periodo { get; set; }

    /// <summary>
    /// Nota relacionada.
    /// </summary>
    public long? NotaRelacionadaId { get; set; }

    /// <summary>
    /// Plano de manutencao associado (usado em preventivas).
    /// </summary>
    [MaxLength(40)]
    public long? PlanoManutencaoCodigo { get; set; }

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
    /// Etiqueta da nota associada.
    /// </summary>
    [MaxLength(80)]
    public string? EtiquetaNota { get; set; }

    /// <summary>
    /// Data de criacao da ordem.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Analise de criticidade.
    /// </summary>
    [MaxLength(120)]
    public string? AnaliseCriticidade { get; set; }

    /// <summary>
    /// Analise de risco operacional.
    /// </summary>
    [MaxLength(120)]
    public string? AnaliseRisco { get; set; }

    /// <summary>
    /// Indicador ARO/ADTC.
    /// </summary>
    [MaxLength(60)]
    public string? AroAdtc { get; set; }

    /// <summary>
    /// Matriz de LIBRA utilizada.
    /// </summary>
    [MaxLength(80)]
    public string? MatrizLibra { get; set; }

    /// <summary>
    /// Permissao de trabalho relacionada.
    /// </summary>
    [MaxLength(120)]
    public string? PermissaoTrabalho { get; set; }

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
    /// Equipamento associado à ordem.
    /// </summary>
    [ForeignKey(nameof(EquipamentoTag))]
    public virtual EquipamentoAtivoModel? Equipamento { get; set; }

    /// <summary>
    /// Local de instalação associado à ordem.
    /// </summary>
    [ForeignKey(nameof(LocalInstalacaoCodigo))]
    public virtual LocalInstalacaoModel? LocalInstalacao { get; set; }

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

