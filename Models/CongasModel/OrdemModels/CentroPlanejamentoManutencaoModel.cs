using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class CentroPlanejamentoManutencaoModel
{
    /// <summary>
    /// Identificador interno do centro de planejamento de manutencao.
    /// </summary>
    [Key]
    public int IdCpm { get; set; }

    /// <summary>
    /// Nome do centro de planejamento de manutencao.
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Organização do centro de planejamento de manutencao no SAP.
    /// </summary>
    [MaxLength(30)]
    public required string Organizacao { get; set; }

    /// <summary>
    /// Localizacao do centro de planejamento de manutencao.
    /// </summary>
    public string? Localizacao { get; set; }

    /// <summary>
    /// Identificador da localizacao vinculada ao CPM.
    /// </summary>
    public int? LocalizacaoId { get; set; }

    /// <summary>
    /// Descricao do centro de planejamento de manutencao.
    /// </summary>
    [MaxLength(150)]
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Centros de trabalho vinculados ao centro de planejamento de manutencao.
    /// </summary>
    public virtual ICollection<CentroTrabalhoModel> CentrosTrabalho { get; set; } = new List<CentroTrabalhoModel>();
    
    /// <summary>
    /// Grupos de planejamento vinculados ao centro de planejamento de manutencao.
    /// </summary>
    public virtual ICollection<GrupoPlanejamentoManutencaoModel> GruposPlanejamento { get; set; } = new List<GrupoPlanejamentoManutencaoModel>();

    /// <summary>
    /// Localizacao vinculada ao CPM.
    /// </summary>
    [ForeignKey(nameof(LocalizacaoId))]
    public virtual LocalizacaoModel? LocalizacaoReferencia { get; set; }
}