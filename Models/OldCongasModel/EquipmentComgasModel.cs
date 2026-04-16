// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using app_pm_back.Attributes;
// using app_pm_back.Models.CongasModel;
//
// namespace app_pm_back.Models;
//
// /// <summary>
// /// Classe de equipamentos, 
// /// </summary>
// public class EquipmentComgasModel
// {
//     /// <summary>
//     /// Identificação do equipamento
//     /// </summary>
//     /// <example>CG_05_EST_ECS_ECS00001</example>
//     [Key]
//     [SheetColumn("Objeto")]
//     public string IdEquipamento { get; set; }
//     
//     /// <summary>
//     /// Atividade do equipamento
//     /// </summary>
//     [SheetColumn("ATIVIDADE")]
//     public string? Atividade { get; set; }
//
//     /// <summary>
//     /// Base de atendimento do equipamento
//     /// </summary>
//     /// <example>JUNDIAI</example>
//     [SheetColumn("BASE DE ATENDIMENTO")] 
//     public string? BaseDeAtendiemnto { get; set; }
//
//     /// <summary>
//     /// Capacidade do equipamento
//     /// </summary>
//     [SheetColumn("CAPACIDADE")]
//     public string? Capacidade { get; set; }
//
//     /// <summary>
//     /// Classe do equipamento
//     /// </summary>
//     [SheetColumn("Classe")]
//     public string? Classe { get; set; }
//         
//     /// <summary>
//     /// Coordenada X do equipamento
//     /// </summary>
//     [SheetColumn("COORDENADAS X")]
//     public string? CoordenadasX { get; set; }
//
//     /// <summary>
//     /// Coordenada Y do equipamento
//     /// </summary>
//     [SheetColumn("COORDENADAS Y")]
//     public string? CoordenadasY { get; set; }
//
//     /// <summary>
//     /// ID do data logger
//     /// </summary>
//     [SheetColumn("DATA LOGGER ID")]
//     public string? DataLoggerId { get; set; }
//
//     /// <summary>
//     /// DDD do equipamento
//     /// </summary>
//     [SheetColumn("DDD")]
//     public string? DDD { get; set; }
//
//     /// <summary>
//     /// Denominação do equipamento
//     /// </summary>
//     [SheetColumn("Denominação")]
//     public string? Denominacao { get; set; }
//
//     /// <summary>
//     /// Elemento PEP do equipamento
//     /// </summary>
//     [SheetColumn("ELEMENTO PEP")]
//     public string? ElementoPep { get; set; }
//
//     /// <summary>
//     /// Fabricante do equipamento
//     /// </summary>
//     [SheetColumn("FABRICANTE")]
//     public string? Fabricante { get; set; }
//     
//     /// <summary>
//     /// Frequência de manutenção do equipamento
//     /// </summary>
//     [SheetColumn("FREQUENCIA DE MANUTENCAO")]
//     public string? FrequenciaDeManutencao { get; set; }
//
//     /// <summary>
//     /// Grid do equipamento
//     /// </summary>
//     [SheetColumn("GRID")]
//     public string? Grid { get; set; }
//
//     /// <summary>
//     /// ID GeoGas do equipamento
//     /// </summary>
//     [SheetColumn("ID GEOGAS")]
//     public string? IdGeogas { get; set; }
//
//     /// <summary>
//     /// Iluminação do equipamento
//     /// </summary>
//     [SheetColumn("ILUMINACAO")]
//     public string? Iluminacao { get; set; }
//
//     /// <summary>
//     /// Material do equipamento
//     /// </summary>
//     [SheetColumn("MATERIAL")]
//     public string? Material { get; set; }
//
//     /// <summary>
//     /// Número da estação interligada
//     /// </summary>
//     [SheetColumn("NUMERO DA ESTACAO INTERLIGADA")]
//     public string? NumeroDaEstacaoInterligada { get; set; }
//
//     /// <summary>
//     /// Número de linhas do equipamento
//     /// </summary>
//     [SheetColumn("NUMERO DE LINHAS")]
//     public int? NumeroDeLinhas { get; set; }
//
//     /// <summary>
//     /// Número do projeto
//     /// </summary>
//     [SheetColumn("NUMERO DO PROJETO")]
//     public string? NumeroDoProjeto { get; set; }
//     
//     /// <summary>
//     /// Ponto de referência do equipamento
//     /// </summary>
//     [SheetColumn("PONTO DE REFERENCIA")]
//     public string? PontoDeReferencia { get; set; }
//     
//     /// <summary>
//     /// Seção do equipamento
//     /// </summary>
//     [SheetColumn("SECAO")]
//     public string? Secao { get; set; }
//
//     /// <summary>
//     /// Status do equipamento
//     /// </summary>
//     [SheetColumn("Status")]
//     public int? Status { get; set; }
//
//     /// <summary>
//     /// Subsistema do equipamento
//     /// </summary>
//     [SheetColumn("SUBSISTEMA")]
//     public string? Subsistema { get; set; }
//
//     /// <summary>
//     /// Tipo do equipamento
//     /// </summary>
//     [SheetColumn("TIPO")]
//     public string? Tipo { get; set; }
//
//     /// <summary>
//     /// Tipo de objeto do equipamento
//     /// </summary>
//     [SheetColumn("Tipo de objeto")]
//     public string? TipoDeObjeto { get; set; }
//
//     /// <summary>
//     /// Data de assentamento do equipamento
//     /// </summary>
//     [SheetColumn("DATA DE ASSENTAMENTO")]
//     public string? DataDeAssentamento { get; set; }
//
//     /// <summary>
//     /// Data de comissionamento do equipamento
//     /// </summary>
//     [SheetColumn("DATA DE COMISSIONAMENTO")]
//     public string? DataDeComissionamento { get; set; }
//
//     /// <summary>
//     /// Data de fabricação do equipamento
//     /// </summary>
//     [SheetColumn("DATA DE FABRICACAO")]
//     public DateTime? DataDeFabricacao { get; set; }
//
//     /// <summary>
//     /// Data de início de operação do equipamento
//     /// </summary>
//     [SheetColumn("DATA DE INICIO DE OPER.")]
//     public DateTime? DataDeInicioDeOper { get; set; }
//
//     /// <summary>
//     /// Data de instalação do equipamento
//     /// </summary>
//     [SheetColumn("DATA DE INSTALACAO")]
//     public DateTime? DataDeInstalacao { get; set; }
//
//     /// <summary>
//     /// Data de fim de operação do equipamento
//     /// </summary>
//     [SheetColumn("DATA FIM DE OPERACAO")]
//     public DateTime? DataFimDeOperacao { get; set; }
//
//     /// <summary>
//     /// Guarda atributos específicos dos equipamentos 
//     /// </summary>
//     /// <Example>{"Material": "Aço"}</Example>
//     [Column(TypeName = "jsonb")]
//     public Dictionary<string, string> DetalhesTecnicos { get; set; } = new Dictionary<string, string>();
//     //var equipamentosAco = await _context.Equipamentos
//     // .Where(e => e.Classe == "ECP/P" && e.DetalhesTecnicos["MATERIAL"] == "ACO")
//     // .ToListAsync();
//     
//     /// <summary>
//     /// Um equipamento pode ter várias Notas Congas ligadas a ele
//     /// </summary>
//     public virtual ICollection<NoteCongasModel>? Notas { get; set; }
// }
//
