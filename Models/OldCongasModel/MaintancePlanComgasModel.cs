// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using app_pm_back.Attributes;
//
// namespace app_pm_back.Models.CongasModel;
//
// /// <summary>
// /// Plano de manutencao legado COMGAS.
// /// </summary>
// public class MaintancePlanComgasModel
// {
//     /// <summary>
//     /// Id do plano de manutenção
//     /// </summary>
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     public int IdPlano { get; set; }
//
//     /// <summary>
//     /// Plano de manutenção
//     /// </summary>
//     /// <example>49643</example>
//     [SheetColumn("Plano manut.")]
//     public long? PlanoManut { get; set; }
//
//     /// <summary>
//     /// Data de emissão do plano
//     /// </summary>
//     /// <example>DD/MM/AAAA</example>
//     [SheetColumn("Data do PM")]
//     public DateTime? DataPM { get; set; }
//
//     /// <summary>
//     /// Chave estrangeira para estratégia 
//     /// </summary>
//     public int? IdEstrategia { get; set; }
//
//     /// <summary>
//     /// Estrategia associada ao plano de manutencao.
//     /// </summary>
//     [ForeignKey("IdEstrategia")]
//     public virtual EstrategiaComgasModel? Estrategia { get; set; }
//
//     // TABELA DE CICLOS (Herdada da estratégia)
//
//     /// <summary>
//     /// Ciclos
//     /// </summary>
//     /// <example>12</example>
//     [SheetColumn("Ciclo")]
//     public int Ciclo { get; set; }
//
//     /// <summary>
//     /// Unidade de medida dos ciclos
//     /// </summary>
//     /// <example>MES</example>
//     [MaxLength(20)]
//     [SheetColumn("Unidade")]
//     public required string Unidade { get; set; }
//
//     /// <summary>
//     /// Campo de texto para ciclo de manutenção
//     /// </summary>
//     /// <example>NIVEL - B</example>
//     [MaxLength(80)]
//     [SheetColumn("Txt.p/ciclo manut.")]
//     public string? TxtCicloManut { get; set; }
//
//     /// <summary>
//     /// Offset dos ciclos
//     /// </summary>
//     [SheetColumn("Offset")]
//     public int? Offset { get; set; }
//
//     /// <summary>
//     /// Itens de manutencao vinculados ao plano.
//     /// </summary>
//     public virtual ICollection<ItemManutencaoComgasModel> ItensManutencao { get; set; } = new List<ItemManutencaoComgasModel>();
//
// }