using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Informacoes de localizacao administrativa e operacional.
/// </summary>
public class LocalizacaoModel
{
    /// <summary>
    /// Identificador interno da localizacao.
    /// </summary>
    [Key]
    public int IdLocalizacao { get; set; }

    /// <summary>
    /// Nome amigavel da localizacao.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }
    

    /// <summary>
    /// Cenario de localizacao no SAP.
    /// </summary>
    [MaxLength(50)]
    public string? CenLocalizacao { get; set; }

    /// <summary>
    /// Codigo da localizacao.
    /// </summary>
    [MaxLength(60)]
    public required string CodigoLocalizacao { get; set; }

    /// <summary>
    /// Indica se o local esta coberto por garantia.
    /// </summary>
    public bool? EmGarantia { get; set; }

    /// <summary>
    /// Area operacional.
    /// </summary>
    [MaxLength(80)]
    public string? AreaOperacional { get; set; }

    /// <summary>
    /// Codigo ABC para classificacao.
    /// </summary>
    [MaxLength(20)]
    public string? CodigoAbc { get; set; }

    /// <summary>
    /// Campo textual de ordenacao utilizado em avisos.
    /// </summary>
    [MaxLength(150)]
    public string? CampoOrdenacao { get; set; }

    /// <summary>
    /// FK do endereco principal.
    /// </summary>
    public int? EnderecoId { get; set; }

    /// <summary>
    /// Endereco principal da localizacao.
    /// </summary>
    [ForeignKey(nameof(EnderecoId))]
    public virtual EnderecoModel? Endereco { get; set; }

    /// <summary>
    /// Locais de instalacao vinculados a esta localizacao.
    /// </summary>
    public virtual ICollection<LocalInstalacaoModel> LocaisInstalacao { get; set; } = new List<LocalInstalacaoModel>();

    /// <summary>
    /// Equipamentos vinculados a esta localizacao.
    /// </summary>
    public virtual ICollection<EquipamentoAtivoModel> Equipamentos { get; set; } = new List<EquipamentoAtivoModel>();
}

