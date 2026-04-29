using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models;

/// <summary>
/// tabela para armazenar anexos 
/// </summary>
public class AnnexModel
{
    /// <summary>
    /// Id do anexo 
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdAnexo { get; set; }
    
    /// <summary>
    /// Nome do anexo
    /// </summary>
    public string NomeAnexo { get; set; }
    
    /// <summary>
    /// MimeType do anexo, usado para identificar o tipo de arquivo 
    /// </summary>
    public string MimeType { get; set; } 
    
    /// <summary>
    /// Conteúdo físico do arquivo
    /// </summary>
    public byte[] Dados { get; set; }
    
    /// <summary>
    /// Guarda o id da referência (NumeroNota, IdOrdem...)
    /// </summary>
    public long ReferenciaId { get; set; }
    
    // identificar a qual entidade o anexo está associado (NOTA, ORDEM...)
    
    /// <summary>
    /// Qual o tipo de referência o anexo é associado (NOTA_CONGAS, ORDEM_CONGAS...)
    /// </summary>
    public string TipoReferencia { get; set; }
    
    /// <summary>
    /// Data de upload do anexo
    /// </summary>
    public DateTime DataUpload { get; set; }
}