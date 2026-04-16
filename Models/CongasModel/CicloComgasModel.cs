using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Ciclo legado da estrategia de manutencao COMGAS.
/// </summary>
public class CicloComgasModel
{
    /// <summary>
    /// Identificador interno do ciclo.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdCiclo { get; set; }

    /// <summary>
    /// Valor numerico do ciclo.
    /// </summary>
    [SheetColumn("Ciclo")]
    public int? ValorCiclo { get; set; }

    /// <summary>
    /// Unidade do ciclo (DIA, MES, ANO).
    /// </summary>
    [MaxLength(20)]
    [SheetColumn("Unidade")]
    public string? Unidade { get; set; }

    /// <summary>
    /// Nível de manutencao do ciclo (A, B ou C).
    /// </summary>
    [MaxLength(5)]
    [SheetColumn("Nível manutenção")]
    public string? NivelManutencao { get; set; }

    /// <summary>
    /// Offset de execucao do ciclo.
    /// </summary>
    [SheetColumn("Offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// FK da estrategia dona do ciclo.
    /// </summary>
    public int? IdEstrategia { get; set; }

    /// <summary>
    /// Estrategia vinculada a este ciclo.
    /// </summary>
    [ForeignKey(nameof(IdEstrategia))]
    public virtual EstrategiaComgasModel? Estrategia { get; set; }
}