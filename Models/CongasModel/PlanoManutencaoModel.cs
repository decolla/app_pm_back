using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Plano de manutencao que organiza ciclos e itens para equipamentos.
/// </summary>
public class PlanoManutencaoModel
{
    /// <summary>
    /// Codigo do plano de manutencao.
    /// </summary>
    [Key]
    [MaxLength(40)]
    public required long CodigoPlano { get; set; }

    /// <summary>
    /// Numero da solicitacao relacionada.
    /// </summary>
    [MaxLength(40)]
    public string? NumeroSolicitacao { get; set; }
    
    /// <summary>
    /// Data de criação do plano.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Data de conclusao do plano.
    /// </summary>
    public DateTime? DataConclusao { get; set; }

    /// <summary>
    /// Ultima ordem gerada por este plano.
    /// </summary>
    public long? UltimaOrdem { get; set; }

    /// <summary>
    /// FK da estrategia associada.
    /// </summary>
    public int? EstrategiaId { get; set; }

    /// <summary>
    /// Estrategia associada ao plano.
    /// </summary>
    [ForeignKey(nameof(EstrategiaId))]
    public virtual EstrategiaManutencaoModel? Estrategia { get; set; }

    /// <summary>
    /// Ciclos previstos para o plano.
    /// </summary>
    public virtual ICollection<CicloManutencaoModel> Ciclos { get; set; } = new List<CicloManutencaoModel>();

    /// <summary>
    /// Itens de manutencao do plano.
    /// </summary>
    public virtual ICollection<ItemPlanoManutencaoModel> Itens { get; set; } = new List<ItemPlanoManutencaoModel>();

    /// <summary>
    /// Ordens vinculadas ao plano.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> Ordens { get; set; } = new List<OrdemManutencaoModel>();
}

