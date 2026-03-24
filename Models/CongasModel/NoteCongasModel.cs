using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Modelo para NotaCongasModel
/// </summary>
public class NoteCongasModel
{
    /// <summary>
    /// Id da Nota
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdNota { get; set; }
    
    /// <summary>
    /// Número da nota vindo do SAP/Planilha
    /// </summary>
    /// <example>4088673</example>
    public long NumeroNota { get; set; }
    
    /// <summary>
    /// Tipo de nota gerada
    /// </summary>
    /// <example>MC</example>
    [Required(ErrorMessage = "O tipo de nota é obrigatório")]
    [Range(1, 2, ErrorMessage = "O tipo de nota deve estar entre 1 e 2")]
    public string TipoNota { get; set; }
    
    /// <summary>
    /// Local de instalação do campo de seleção
    /// </summary>
    /// <example>CG_05_CON_IND_CRM00087</example>
    [Required(ErrorMessage = "O Local de instalação é obrigatório")]
    public string LocInstalacao { get; set; }
    
    /// <summary>
    /// Status do estado do ativo relatado na nota
    /// </summary>
    /// <example> MSEN </example>
    [Required(ErrorMessage = "O status é obrigatório")]
    [Range(1,4, ErrorMessage = "O status deve estar entre 1 e 4")]
    public string StatusSistema { get; set; }
    
    /// <summary>
    /// Descrição do estado do ativo relatado na nota
    /// </summary>
    /// <example> APRV </example>
    [Required(ErrorMessage = "O status é obrigatório")]
    [Range(1,4, ErrorMessage = "O status deve estar entre 1 e 4")]
    public string StatusUsuario { get; set; }
    
    /// <summary>
    /// Descrição do estado do ativo relatado na nota
    /// </summary>
    /// <example>UAR 2014 - SVVA TROCA DA VALVULA ALIVIO</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }

    /// <summary>
    /// Rua da localização
    /// </summary>
    /// <example>R PASCAL 000000</example>
    public string Rua { get; set; }
    
    /// <summary>
    /// Rua da localização
    /// </summary>
    /// <example>CAMPO BELO</example>
    public string Bairro { get; set; }
    
    /// <summary>
    /// Rua da localização
    /// </summary>
    /// <example>SAO PAULO</example>
    public string Local { get; set; }
    
    /// <summary>
    /// Data de emissão da nota 
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [Required(ErrorMessage = "A data é obrigatória")]
    public DateTime DataNota { get; set; }
    
    /// <summary>
    /// Data de modificação da nota 
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    public DateTime? ModificadoEm { get; set; }
    
    /// <summary>
    /// Identificação de quem modificou a nota
    /// </summary>
    /// <example>CS123456</example>
    public string? ModificadoPor { get; set; }
    
    /// <summary>
    /// Horário de modificação da nota
    /// </summary>
    /// <example>hh:mm:ss</example>
    public DateTime? ModificadoAs { get; set; }
    
    /// <summary>
    /// Tipo de prioridade da nota (PM ou EM)
    /// </summary>
    /// <example>PM</example>
    [Required(ErrorMessage = "O tipo de prioridade da nota é obrigatório")]
    public string TipoPrioridade { get; set; }
    
    /// <summary>
    /// Data de início da avaria
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    public DateTime? InicioAvar { get; set; }
    
    /// <summary>
    /// Data de fim da avaria
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    public DateTime? FimAvaria { get; set; }
    
    /// <summary>
    /// Hora de início da avaria
    /// </summary>
    /// <example>hh:mm:ss</example>
    public DateTime? HoraInicioAvaria { get; set; }

    /// <summary>
    /// Hora do fim da avaria
    /// </summary>
    /// <example>hh:mm:ss</example>
    public DateTime? HoraFimAvaria { get; set; }

    /// <summary>
    /// Data de referência
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    public DateTime? DtReferencia { get; set; }

    /// <summary>
    /// Hora de referência
    /// </summary>
    /// <example>hh:mm:ss</example>
    public DateTime? HoraReferencia { get; set; }

    /// <summary>
    /// Indicador de se terá parada
    /// </summary>
    /// <example>X</example>
    public string? Parada { get; set; }

    /// <summary>
    /// Duração da parada
    /// </summary>
    public decimal? DuracParada { get; set; }

    /// <summary>
    /// Data de criação da nota
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    public DateTime? DtCriacao { get; set; }

    /// <summary>
    /// Usuário que criou a nota
    /// </summary>
    /// <example>CS123456</example>
    public string? CriadoPor { get; set; }

    /// <summary>
    /// Hora em que a nota foi criada
    /// </summary>
    /// <example>hh:mm:ss</example>
    public DateTime? CriadoAs { get; set; }

    /// <summary>
    /// Código do notificador
    /// </summary>
    /// <example>OOOM1</example>
    public string? Notificador { get; set; }

    /// <summary>
    /// Perfil do catálogo
    /// </summary>
    /// <example>PM_COR_02</example>
    public string? PerfCatalogo { get; set; }

    /// <summary>
    /// Centro de trabalho responsável
    /// </summary>
    /// <example>CGP0101</example>
    public string? CenTrabRespon { get; set; }

    /// <summary>
    /// Texto do code de codificação
    /// </summary>
    /// <example>ERD</example>
    public string? TxtCodeCodif { get; set; }

    /// <summary>
    /// Texto do grupo de codificação
    /// </summary>
    /// <example>NOTA DE MANUTENÇÃO</example>
    public string? TxtGrpCodific { get; set; }

    /// <summary>
    /// Local de instalação afetado
    /// </summary>
    /// <example>CG_08_EST_ECP_ECP00066</example>
    public string? LocInstAfetado { get; set; }

    /// <summary>
    /// Equipamento envolvido na nota
    /// </summary>
    /// <example>123456789</example>
    public string? Equipamento { get; set; }

    /// <summary>
    /// Denominação do ativo
    /// </summary>
    /// <example>ERP123</example>
    public string? Denominacao { get; set; }

    /// <summary>
    /// Grupo de planejamento PM
    /// </summary>
    /// <example>100</example>
    public string? GrpPlnjPM { get; set; }

    /// <summary>
    /// Área operacional
    /// </summary>
    /// <example>019</example>
    public string? AreaOperacion { get; set; }

    /// <summary>
    /// Codificação do problema
    /// </summary>
    /// <example>N1</example>
    public string? Codificacao { get; set; }

    /// <summary>
    /// Nível de consequência do problema, de 1 a 4
    /// </summary>
    /// <example>2</example>
    [Range(1,4,ErrorMessage = "O limite do nível é de 1 a 4")]
    public int? Consequencia { get; set; }

    /// <summary>
    /// Grupo de codes
    /// </summary>
    /// <example>01-NOTA</example>
    public string? GrupoCodes { get; set; }

    /// <summary>
    /// ID da ordem gerada por esta nota 
    /// <example>200000083031</example>
    public long? IdOrdem { get; set; }

}