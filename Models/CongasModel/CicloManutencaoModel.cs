using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Ciclo de manutencao de um plano.
/// </summary>
public class CicloManutencaoModel
{
    /// <summary>
    /// Identificador interno do ciclo.
    /// </summary>
    [Key]
    public int IdCiclo { get; set; }

    /// <summary>
    /// Valor numerico do ciclo.
    /// </summary>
    public int Valor { get; set; }

    /// <summary>
    /// Unidade do ciclo (DIA, MES, ANO).
    /// </summary>
    [MaxLength(10)]
    public required string Unidade { get; set; }

    /// <summary>
    /// Nivel de manutencao (A, B ou C).
    /// </summary>
    [MaxLength(1)]
    public string? NivelManutencao { get; set; }

    /// <summary>
    /// Offset do ciclo.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// Codigo do plano de manutencao.
    /// </summary>
    [MaxLength(40)]
    public required long PlanoManutencaoCodigo { get; set; }

    /// <summary>
    /// Plano de manutencao associado.
    /// </summary>
    [ForeignKey(nameof(PlanoManutencaoCodigo))]
    public virtual PlanoManutencaoModel PlanoManutencao { get; set; } = null!;
}

