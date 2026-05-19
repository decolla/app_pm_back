using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Almoxarifado com estoque de kits e ferramentas.
/// </summary>
public class AlmoxarifadoModel
{
    /// <summary>
    /// Identificador interno do almoxarifado.
    /// </summary>
    [Key]
    public int IdAlmoxarifado { get; set; }

    /// <summary>
    /// Nome do almoxarifado.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Organizacao do almoxarifado.
    /// </summary>
    [MaxLength(80)]
    public required string Organizacao { get; set; }

    /// <summary>
    /// Localizacao vinculada ao almoxarifado.
    /// </summary>
    public int? LocalizacaoId { get; set; }

    /// <summary>
    /// Status do almoxarifado.
    /// </summary>
    [MaxLength(30)]
    public string? Status { get; set; }

    /// <summary>
    /// Descricao do almoxarifado.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Localizacao associada.
    /// </summary>
    [ForeignKey(nameof(LocalizacaoId))]
    public virtual LocalizacaoModel? Localizacao { get; set; }

    /// <summary>
    /// Funcionarios vinculados ao almoxarifado.
    /// </summary>
    public virtual ICollection<FuncionarioModel> Funcionarios { get; set; } = new List<FuncionarioModel>();

    /// <summary>
    /// Kits e ferramentas disponíveis no almoxarifado.
    /// </summary>
    public virtual ICollection<FerramentasTipoModel> Ferramentas { get; set; } = new List<FerramentasTipoModel>();
}

