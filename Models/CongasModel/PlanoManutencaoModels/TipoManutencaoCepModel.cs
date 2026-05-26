using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

public class TipoManutencaoCepModel
{
    /// <summary>
    /// Id do tipo de manutencao.
    /// </summary>
    [Key]
    public int IdTipoManutencaoCep { get; set; }

    /// <summary>
    /// Tipo de ativo a ser aplicado.
    /// </summary>
    [MaxLength(80)]
    public string TipoAtivo { get; set; } = string.Empty;

    /// <summary>
    /// Confiabilidade em porcentagem para o tipo de manutencao.
    /// </summary>
    [Range(0, 100)]
    public int Confiabilidade { get; set; }

    /// <summary>
    /// Se o tipo de manutencao esta habilitado.
    /// </summary>
    public bool Habilitada { get; set; } = true;

    /// <summary>
    /// Selecao de niveis e descricoes.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, int>? Niveis { get; set; }

    /// <summary>
    /// Acoes selecionadas em caso de falha.
    /// </summary>
    /// <example>AbrirNota = 0, AbrirOrdem = 1, OpcaoTemplate = 2, Nada = 3</example>
    [Column(TypeName = "jsonb")]
    public List<AcoesFalha> AcoesFalha { get; set; } = [Models.CongasModel.PlanoManutencaoModels.ManutencaoModels.AcoesFalha.Nada];
    
    /// <summary>
    /// Guarda os tipos de estrategia associados a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<TipoEstrategiaModel> TiposEstrategias { get; set; }
    
    /// <summary>
    /// Guarda os tipos de plano associados a este tipo de manutencao.
    /// </summary>
    public virtual ICollection<TipoPlanoModel> TipoPlanoModels { get; set; }
}

public enum AcoesFalha
{
    AbrirNota = 0,
    AbrirOrdem = 1,
    OpcaoTemplate = 2,
    Nada = 3
}