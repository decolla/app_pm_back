using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Local de instalacao que agrupa equipamentos e relacoes operacionais.
/// </summary>
public class LocalInstalacaoModel
{
    /// <summary>
    /// Codigo do local de instalacao no padrao SAP.
    /// </summary>
    [Key]
    [MaxLength(60)]
    public required string CodigoLocalInstalacao { get; set; }

    /// <summary>
    /// Denominacao do local.
    /// </summary>
    [MaxLength(200)]
    public required string Denominacao { get; set; }

    /// <summary>
    /// Status atual do local.
    /// </summary>
    [MaxLength(20)]
    public string? Status { get; set; }

    /// <summary>
    /// Sistema identificado.
    /// </summary>
    [MaxLength(80)]
    public string? SistemaIdentificado { get; set; }

    /// <summary>
    /// Codigo da estrutura tecnica.
    /// </summary>
    [MaxLength(60)]
    public string? CodigoEstrutura { get; set; }

    /// <summary>
    /// Categoria do local.
    /// </summary>
    [MaxLength(50)]
    public string? Categoria { get; set; }

    /// <summary>
    /// Classe tecnica.
    /// </summary>
    [MaxLength(80)]
    public string? Classe { get; set; }

    /// <summary>
    /// Tipo de objeto.
    /// </summary>
    [MaxLength(50)]
    public string? TipoObjeto { get; set; }

    /// <summary>
    /// Grupo de autorizacoes.
    /// </summary>
    [MaxLength(50)]
    public string? GrupoAutorizacoes { get; set; }

    /// <summary>
    /// Peso do ativo do local.
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
    /// Fabricante associado ao local.
    /// </summary>
    public int? FabricanteId { get; set; }

    /// <summary>
    /// Localizacao associada.
    /// </summary>
    public int? LocalizacaoId { get; set; }

    /// <summary>
    /// Endereco associado.
    /// </summary>
    public int? EnderecoId { get; set; }

    /// <summary>
    /// Imobilizado associado.
    /// </summary>
    [MaxLength(60)]
    public string? ImobilizadoTag { get; set; }

    /// <summary>
    /// Grupo de planejamento de manutencao associado.
    /// </summary>
    public int? GpmId { get; set; }

    /// <summary>
    /// Centro de trabalho associado.
    /// </summary>
    public int? CentroTrabalhoId { get; set; }

    /// <summary>
    /// Fabricante do local.
    /// </summary>
    [ForeignKey(nameof(FabricanteId))]
    public virtual FabricanteModel? Fabricante { get; set; }

    /// <summary>
    /// Localizacao do local.
    /// </summary>
    [ForeignKey(nameof(LocalizacaoId))]
    public virtual LocalizacaoModel? Localizacao { get; set; }

    /// <summary>
    /// Endereco do local.
    /// </summary>
    [ForeignKey(nameof(EnderecoId))]
    public virtual EnderecoModel? Endereco { get; set; }

    /// <summary>
    /// Imobilizado do local.
    /// </summary>
    [ForeignKey(nameof(ImobilizadoTag))]
    public virtual ImobilizadoModel? Imobilizado { get; set; }

    /// <summary>
    /// Grupo de planejamento do local.
    /// </summary>
    [ForeignKey(nameof(GpmId))]
    public virtual GrupoPlanejamentoManutencaoModel? Gpm { get; set; }

    /// <summary>
    /// Centro de trabalho do local.
    /// </summary>
    [ForeignKey(nameof(CentroTrabalhoId))]
    public virtual CentroTrabalhoModel? CentroTrabalho { get; set; }

    /// <summary>
    /// Equipamentos instalados neste local.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();

    /// <summary>
    /// Notas de manutencao abertas para este local.
    /// </summary>
    public virtual ICollection<NotaManutencaoModel> Notas { get; set; } = new List<NotaManutencaoModel>();

    /// <summary>
    /// Itens de manutencao associados ao local.
    /// </summary>
    public virtual ICollection<ItemPlanoManutencaoModel> ItensPlano { get; set; } = new List<ItemPlanoManutencaoModel>();

    /// <summary>
    /// Ordens vinculadas a este local de instalação.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> Ordens { get; set; } = new List<OrdemManutencaoModel>();
}

