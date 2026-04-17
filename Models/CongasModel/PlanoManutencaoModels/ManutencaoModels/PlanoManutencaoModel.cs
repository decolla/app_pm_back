using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.CongasModel;

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
    /// Nome do plano de manutencao.
    /// </summary>
    [MaxLength(150)]
    public string NomePlano { get; set; }
    
    /// <summary>
    /// Descricao do plano de manutencao.
    /// </summary>
    [MaxLength(500)]
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Nível de prioridade do plano.
    /// </summary>
    /// <example> Baixa = 1,
    /// BaixaMedia = 2,
    /// Media = 3,
    /// MediaAlta = 4,
    /// Alta = 5 </example>
    public Prioridade? Prioridade{ get; set; }
    
    /// <summary>
    /// Gerar automaticamente nota, ordem ou ambos.
    /// </summary>
    /// <example> Nenhum = 0,
    /// Nota = 1,
    /// Ordem = 2,
    /// Ambos = 3
    /// </example>
    public GerarAutomaticamente? GerarAutomaticamente { get; set; }
    
    /// <summary>
    /// Data de criação do plano.
    /// </summary>
    public DateTime? DataCriacao { get; set; }

    /// <summary>
    /// Data de conclusao do plano.
    /// </summary>
    public DateTime? DataConclusao { get; set; }
    
    /// <summary>
    /// Antencedencia do plano em dias.
    /// </summary>
    public int Antencedencia { get; set; }

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
    /// Tipos de manutencao deste plano.
    /// </summary>
    public virtual ICollection<TipoManutencaoModel> TiposManutencao { get; set; } = new List<TipoManutencaoModel>();

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

/// <summary>
/// Nivel de prioridade do plano.
/// </summary>
public enum Prioridade
{
    Baixa = 1,
    BaixaMedia = 2,
    Media = 3,
    MediaAlta = 4,
    Alta = 5
}

/// <summary>
/// Gerar automaticamente nota, ordem ou ambos.
/// </summary>
public enum GerarAutomaticamente
{
    Nenhum = 0,
    Nota = 1,
    Ordem = 2,
    Ambos = 3
}

