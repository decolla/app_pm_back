using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

public class MaintancePlanComgasModel
{
    /// <summary>
    /// Id do plano de manutenção
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPlano { get; set; }
    
    /// <summary>
    /// Plano de manutenção
    /// </summary>
    /// <example>49643</example>
    [SheetColumn("Plano manut.")]
    public long? PlanoManut { get; set; }
    
    /// <summary>
    /// Data de emissão do plano
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Data do PM")]
    public DateTime? DataPM { get; set; }
    
    /// <summary>
    /// Estratégia associada ao plano
    /// </summary>
    [SheetColumn("Estratégia")]
    public string? Estrategia { get; set; }

    // TABELA DE CICLOS (Herdada da estratégia)

    /// <summary>
    /// Ciclos
    /// </summary>
    /// <example>12</example>
    [SheetColumn("Ciclo")]
    public int Ciclo { get; set; }

    /// <summary>
    /// Unidade de medida dos ciclos
    /// </summary>
    /// <example>MES</example>
    [SheetColumn("Unidade")]
    public string Unidade { get; set; }

    /// <summary>
    /// Campo de texto para ciclo de manutenção
    /// </summary>
    /// <example>NIVEL - B</example>
    [SheetColumn("Txt.p/ciclo manut.")]
    public string? TxtCicloManut { get; set; }

    /// <summary>
    /// Offset dos ciclos
    /// </summary>
    [SheetColumn("Offset")]
    public int? Offset { get; set; }


    // TABELA DE ITEM

    /// <summary>
    /// Item de manutenção
    /// </summary>
    [SheetColumn("Item manut.")]
    public long? ItemManut { get; set; }

    /// <summary>
    /// Grupo de planejamento PM
    /// </summary>
    /// <example>100</example>
    [SheetColumn("Grp.plnj.PM")]
    public string? GrpPlnjPM { get; set; }
}