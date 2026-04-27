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
    /// Nome do tipo de manutencao.
    /// </summary>
    [MaxLength(80)]
    public string NomeTipo { get; set; } = string.Empty;

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
    public Dictionary<string, string>? Niveis { get; set; }

    /// <summary>
    /// Acoes selecionadas em caso de falha.
    /// </summary>
    /// <example>AbrirNota = 0, AbrirOrdem = 1, OpcaoTemplate = 2, Nada = 3</example>
    public List<AcoesFalha> AcoesFalha { get; set; } = [Models.CongasModel.PlanoManutencaoModels.ManutencaoModels.AcoesFalha.Nada];

    /// <summary>
    /// FK do plano de manutencao associado.
    /// </summary>
    public long? PlanoManutencaoCodigo { get; set; }

    /// <summary>
    /// Plano de manutencao associado ao tipo.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoCodigo))]
    [JsonIgnore]
    public virtual PlanoManutencaoModel? PlanoManutencao { get; set; }
}

public enum AcoesFalha
{
    AbrirNota = 0,
    AbrirOrdem = 1,
    OpcaoTemplate = 2,
    Nada = 3
}