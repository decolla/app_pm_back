using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

/// <summary>
/// Cadastro de kits e ferramentas reutilizáveis em tipos de manutenção e operações.
/// </summary>
public class FerramentasTipoModel
{
    /// <summary>
    /// Id da ferramenta
    /// </summary>
    [Key]
    public int IdFerramenta { get; set; }
    
    /// <summary>
    /// Nome de identificação da ferramenta
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Categoria do item (kit ou ferramenta).
    /// </summary>
    /// <example>kit</example>
    [MaxLength(30)]
    public string? Categoria { get; set; }

    /// <summary>
    /// Texto breve do item.
    /// </summary>
    [MaxLength(300)]
    public string? TextoBreve { get; set; }

    /// <summary>
    /// Tipo do kit ou ferramenta.
    /// </summary>
    [MaxLength(80)]
    public string? TipoFerramenta { get; set; }
    
    /// <summary>
    /// Quantidade presente no kit ou quantidade total da ferramenta, caso seja unitária.
    /// </summary>
    public int Quantidade { get; set; }

    /// <summary>
    /// Texto detalhado do item.
    /// </summary>
    [MaxLength(1000)]
    public string? TextoDetalhado { get; set; }

    /// <summary>
    /// Descrição da ferramenta
    /// </summary>
    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Centro de trabalho responsável pelo cadastro.
    /// </summary>
    public int? CentroTrabalhoId { get; set; }

    /// <summary>
    /// Local de instalação associado ao kit/ferramenta.
    /// </summary>
    [MaxLength(60)]
    public string? LocalInstalacaoCodigo { get; set; }

    /// <summary>
    /// Fabricante informado para o kit/ferramenta.
    /// </summary>
    public int? FabricanteId { get; set; }

    /// <summary>
    /// Peso bruto do item.
    /// </summary>
    public decimal? PesoBruto { get; set; }

    /// <summary>
    /// Unidade de medida do peso bruto.
    /// </summary>
    [MaxLength(20)]
    public string? PesoBrutoUnidade { get; set; }

    /// <summary>
    /// Peso líquido do item.
    /// </summary>
    public decimal? PesoLiquido { get; set; }

    /// <summary>
    /// Unidade de medida do peso liquido.
    /// </summary>
    [MaxLength(20)]
    public string? PesoLiquidoUnidade { get; set; }

    /// <summary>
    /// Volume do item.
    /// </summary>
    public decimal? Volume { get; set; }

    /// <summary>
    /// Unidade de medida do volume.
    /// </summary>
    [MaxLength(20)]
    public string? VolumeUnidade { get; set; }

    /// <summary>
    /// Dimensão do item.
    /// </summary>
    public decimal? Dimensao { get; set; }

    /// <summary>
    /// Unidade de medida da dimensao.
    /// </summary>
    [MaxLength(20)]
    public string? DimensaoUnidade { get; set; }

    /// <summary>
    /// Valor de aquisição ou referência.
    /// </summary>
    public decimal? Valor { get; set; }

    /// <summary>
    /// Data de aquisicao do item.
    /// </summary>
    public DateTime? DataAquisicao { get; set; }

    /// <summary>
    /// Data de garantia do item.
    /// </summary>
    public DateTime? GarantiaAte { get; set; }
    
    /// <summary>
    /// Quantidade disponível da ferramenta
    /// </summary>
    public int QuantidadeDisp { get; set; }
    
    /// <summary>
    /// Chave estrangeira para TipoManutencao
    /// </summary>
    public int TipoManutencaoId { get; set; }

    /// <summary>
    /// Tipo de manutenção associado ao item.
    /// </summary>
    [ForeignKey(nameof(TipoManutencaoId))]
    public virtual TipoManutencaoModel TipoManutencao { get; set; } = null!;

    /// <summary>
    /// Centro de trabalho associado.
    /// </summary>
    [ForeignKey(nameof(CentroTrabalhoId))]
    public virtual CentroTrabalhoModel? CentroTrabalho { get; set; }

    /// <summary>
    /// Local de instalação associado.
    /// </summary>
    [ForeignKey(nameof(LocalInstalacaoCodigo))]
    public virtual LocalInstalacaoModel? LocalInstalacao { get; set; }

    /// <summary>
    /// Fabricante associado.
    /// </summary>
    [ForeignKey(nameof(FabricanteId))]
    public virtual FabricanteModel? Fabricante { get; set; }

    /// <summary>
    /// Operações que utilizam esta ferramenta ou kit.
    /// </summary>
    public virtual ICollection<OperacaoOrdemModel> Operacoes { get; set; } = new List<OperacaoOrdemModel>();

    /// <summary>
    /// Almoxarifados que armazenam esta ferramenta ou kit.
    /// </summary>
    public virtual ICollection<AlmoxarifadoModel> Almoxarifados { get; set; } = new List<AlmoxarifadoModel>();
}