using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.Domain;

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
    /// Id da possível nota associada ao anexo, caso haja
    /// </summary>
    public long? NotaId { get; set; }
    
    /// <summary>
    /// Nota associada ao anexo.
    /// </summary>
    [ForeignKey(nameof(NotaId))]
    public virtual NotaManutencaoModel? Nota { get; set; }
    
    /// <summary>
    /// Id da possível ordem associada ao anexo, caso haja
    /// </summary>
    public long? OrdemId { get; set; }

    /// <summary>
    /// Ordem associada ao anexo.
    /// </summary>
    [ForeignKey(nameof(OrdemId))]
    public virtual OrdemManutencaoModel? Ordem { get; set; }
    
    /// <summary>
    /// Qual o tipo de referência o anexo é associado (NOTA_CONGAS, ORDEM_CONGAS...)
    /// </summary>
    public string TipoReferencia { get; set; }
    
    /// <summary>
    /// Categoria do anexo
    /// </summary>
    /// <example>AnaLiseRisco = 1</example>
    public CategoriaAnexo Categoria { get; set; } = CategoriaAnexo.Outros;
    
    /// <summary>
    /// Data de upload do anexo
    /// </summary>
    public DateTime DataUpload { get; set; }
}

public enum CategoriaAnexo
{
    Outros = 0,
    AnaliseRisco = 1,
    ARO = 2,
    MatrizLibra = 3,
    PermissaoTrabalho =4,
    Relatorio = 5
    
}