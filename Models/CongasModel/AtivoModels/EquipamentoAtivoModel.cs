using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Equipamento vinculado ao local de instalacao.
/// </summary>
public class EquipamentoAtivoModel
{
    /// <summary>
    /// TAG unica do equipamento.
    /// </summary>
    [Key]
    [MaxLength(60)]
    public string TagEquipamento { get; set; }

    /// <summary>
    /// Tipo de equipamento.
    /// </summary>
    [MaxLength(50)]
    public string? Tipo { get; set; }

    /// <summary>
    /// Número de série do equipamento.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Número do modelo do equipamento.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroModelo { get; set; }

    /// <summary>
    /// Denominação do tipo do equipamento.
    /// </summary>
    [MaxLength(150)]
    public string? DenominacaoTipo { get; set; }

    /// <summary>
    /// Ano e mês de fabricação do equipamento.
    /// </summary>
    [MaxLength(20)]
    public string? AnoMesFabricacao { get; set; }

    /// <summary>
    /// Denominacao do equipamento.
    /// </summary>
    [MaxLength(200)]
    public string? Denominacao { get; set; }

    /// <summary>
    /// Status atual.
    /// </summary>
    [MaxLength(20)]
    public string? Status { get; set; }

    /// <summary>
    /// Data de inicio de validade.
    /// </summary>
    public DateTime? ValidoDesde { get; set; }

    /// <summary>
    /// Data de fim de validade.
    /// </summary>
    public DateTime? ValidoAte { get; set; }

    /// <summary>
    /// Classe tecnica.
    /// </summary>
    [MaxLength(80)]
    public string? Classe { get; set; }

    /// <summary>
    /// Tipo de objeto tecnico.
    /// </summary>
    [MaxLength(50)]
    public string? TipoObjeto { get; set; }

    /// <summary>
    /// Grupo de autorizacoes.
    /// </summary>
    [MaxLength(50)]
    public string? GrupoAutorizacoes { get; set; }

    /// <summary>
    /// Peso do equipamento.
    /// </summary>
    public decimal? Peso { get; set; }

    /// <summary>
    /// Numero de inventario.
    /// </summary>
    [MaxLength(80)]
    public string? NumeroInventario { get; set; }

    /// <summary>
    /// Tipo de nota padrao.
    /// </summary>
    [MaxLength(10)]
    public string? TipoNotaPadrao { get; set; }

    /// <summary>
    /// Valor de aquisicao.
    /// </summary>
    public decimal? ValorAquisicao { get; set; }

    /// <summary>
    /// Data de aquisicao.
    /// </summary>
    public DateTime? DataAquisicao { get; set; }

    /// <summary>
    /// FK do fabricante.
    /// </summary>
    public int? FabricanteId { get; set; }

    /// <summary>
    /// FK da localizacao.
    /// </summary>
    public int? LocalizacaoId { get; set; }

    /// <summary>
    /// FK do endereco.
    /// </summary>
    public int? EnderecoId { get; set; }

    /// <summary>
    /// FK do imobilizado.
    /// </summary>
    [MaxLength(60)]
    public string? ImobilizadoTag { get; set; }

    /// <summary>
    /// FK do grupo de planejamento.
    /// </summary>
    public int? GpmId { get; set; }

    /// <summary>
    /// FK do centro de trabalho.
    /// </summary>
    public int? CentroTrabalhoId { get; set; }

    /// <summary>
    /// FK do local de instalacao.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }

    /// <summary>
    /// Fabricante do equipamento.
    /// </summary>
    [ForeignKey(nameof(FabricanteId))]
    public virtual FabricanteModel? Fabricante { get; set; }

    /// <summary>
    /// Localizacao do equipamento.
    /// </summary>
    [ForeignKey(nameof(LocalizacaoId))]
    public virtual LocalizacaoModel? Localizacao { get; set; }

    /// <summary>
    /// Endereco do equipamento.
    /// </summary>
    [ForeignKey(nameof(EnderecoId))]
    public virtual EnderecoModel? Endereco { get; set; }

    /// <summary>
    /// Imobilizado do equipamento.
    /// </summary>
    [ForeignKey(nameof(ImobilizadoTag))]
    public virtual ImobilizadoModel? Imobilizado { get; set; }

    /// <summary>
    /// Grupo de planejamento do equipamento.
    /// </summary>
    [ForeignKey(nameof(GpmId))]
    public virtual GrupoPlanejamentoManutencaoModel? Gpm { get; set; }

    /// <summary>
    /// Centro de trabalho do equipamento.
    /// </summary>
    [ForeignKey(nameof(CentroTrabalhoId))]
    public virtual CentroTrabalhoModel? CentroTrabalho { get; set; }

    /// <summary>
    /// Local de instalacao onde o equipamento esta instalado.
    /// </summary>
    [ForeignKey(nameof(LocalInstalacaoCodigo))]
    public virtual LocalInstalacaoModel? LocalInstalacao { get; set; }
    
    /// <summary>
     /// Guarda atributos específicos dos equipamentos 
     /// </summary>
     /// <Example>{"Material": "Aço"}</Example>
     [Column(TypeName = "jsonb")]
     public Dictionary<string, string> DetalhesTecnicos { get; set; } = new Dictionary<string, string>();
     //var equipamentosAco = await _context.Equipamentos
     // .Where(e => e.Classe == "ECP/P" && e.DetalhesTecnicos["MATERIAL"] == "ACO")
     // .ToListAsync();
     

    /// <summary>
    /// Notas associadas ao equipamento.
    /// </summary>
    public virtual ICollection<NotaManutencaoModel> Notas { get; set; } = new List<NotaManutencaoModel>();

    /// <summary>
    /// Itens de plano vinculados ao equipamento.
    /// </summary>
    public virtual ICollection<ItemPlanoManutencaoModel> ItensPlano { get; set; } = new List<ItemPlanoManutencaoModel>();

    /// <summary>
    /// Ordens vinculadas a este equipamento.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> Ordens { get; set; } = new List<OrdemManutencaoModel>();

    /// <summary>
    /// Componentes vinculados a este equipamento.
    /// </summary>
    public virtual ICollection<ComponenteEquipamentoModel> Componentes { get; set; } = new List<ComponenteEquipamentoModel>();
}
