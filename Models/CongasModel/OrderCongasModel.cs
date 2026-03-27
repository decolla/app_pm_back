using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Modelo para OrdemCongasModel
/// </summary>
public class OrderCongasModel : IClassifiable
{
    /// <summary>
    /// Número da Ordem (Chave Primária)
    /// </summary>
    /// <example>200000081207</example>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // O ID vem da planilha, não gera automaticamente
    public long IdOrdem { get; set; }
    
        
    /// <summary>
    /// O resultado do tipo da nota
    /// </summary>
    /// <example>CORRETIVA ou PREVENTIVA</example>
    public string? Classificacao { get; set; }
    
    /// <summary>
    /// Quem/o que tomou a decisão da classificação da nota
    /// </summary>
    /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
    public string? MetodoClassificacao { get; set;}
    
    /// <summary>
    /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
    /// </summary>
    public string? ClassificadoPor { get; set; }
    
    /// <summary>
    /// Data de quando a classificacao ocorreu
    /// </summary>
    public DateTime? DataClassificacao { get; set; }

    /// <summary>
    /// Local de instalação do ativo
    /// </summary>
    /// <example>CG_05_CON_IND_CRM00073</example>
    [Required]
    public string LocInstalacao { get; set; }

    /// <summary>
    /// Status do sistema da ordem
    /// </summary>
    /// <example>ENTE CONF DMNV MOME NOLQ</example>
    public string? StatusSistema { get; set; }

    /// <summary>
    /// Status do usuário
    /// </summary>
    /// <example>APRO</example>
    public string? StatusUsuario { get; set; }

    /// <summary>
    /// Texto breve com a descrição da ordem
    /// </summary>
    /// <example>BAUDUCCO (1)</example>
    [Required]
    public string TextoBreve { get; set; }

    /// <summary>
    /// Plano de manutenção atrelado
    /// </summary>
    /// <example>10419</example>
    public string? PlanoManut { get; set; }

    /// <summary>
    /// Item de manutenção
    /// </summary>
    /// <example>36467</example>
    public string? ItemManut { get; set; }

    /// <summary>
    /// Tipo de ordem PM
    /// </summary>
    /// <example>ZM03</example>
    [Required]
    public string TipoOrdem { get; set; }

    /// <summary>
    /// Tipo de atividade PM
    /// </summary>
    /// <example>Z02</example>
    public string? TipoAtividPM { get; set; }

    /// <summary>
    /// Data base de início
    /// </summary>
    /// <example>2009-12-15</example>
    public DateTime? DataBaseInic { get; set; }

    /// <summary>
    /// Data real de início da ordem
    /// </summary>
    /// <example>2010-01-24</example>
    public DateTime? DtaInicioReal { get; set; }

    /// <summary>
    /// Data real de fim da ordem
    /// </summary>
    /// <example>2010-01-24</example>
    public DateTime? DtRealFim { get; set; }

    /// <summary>
    /// Hora real de início
    /// </summary>
    /// <example>11:00:00</example>
    public DateTime? InicRealHr { get; set; }

    /// <summary>
    /// Hora real de fim
    /// </summary>
    /// <example>13:20:00</example>
    public DateTime? FimRealHora { get; set; }

    /// <summary>
    /// Grupo de planejamento PM
    /// </summary>
    /// <example>101</example>
    public string? GrpPlnjPM { get; set; }

    /// <summary>
    /// Centro de trabalho responsável
    /// </summary>
    /// <example>ENGICRM</example>
    public string? CenTrabRespon { get; set; }

    /// <summary>
    /// Denominação do local
    /// </summary>
    /// <example>HOSPITAL SAO LUIZ</example>
    public string? Denominacao { get; set; }

    /// <summary>
    /// Local / Cidade
    /// </summary>
    /// <example>SAO PAULO</example>
    public string? Local { get; set; }

    /// <summary>
    /// Rua
    /// </summary>
    /// <example>AV STO AMARO 000734</example>
    public string? Rua { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    /// <example>VILA ENDRES</example>
    public string? Bairro { get; set; }

    /// <summary>
    /// Área operacional
    /// </summary>
    /// <example>003</example>
    public string? AreaOperacion { get; set; }

    /// <summary>
    /// Usuário criador da ordem
    /// </summary>
    /// <example>IP1020091109</example>
    public string? CriadoPor { get; set; }

    /// <summary>
    /// Data de modificação da ordem
    /// </summary>
    public DateTime? DataModif { get; set; }

    /// <summary>
    /// Usuário que modificou a ordem
    /// </summary>
    public string? ModificadoPor { get; set; }

    /// <summary>
    /// Nota atrelada à ordem (Chave Estrangeira lógica)
    /// </summary>
    /// <example>90003663</example>
    public long? NumeroNotaOrigem { get; set; }

    /// <summary>
    /// Ordem principal vinculada
    /// </summary>
    public string? OrdemPrincip { get; set; }
}