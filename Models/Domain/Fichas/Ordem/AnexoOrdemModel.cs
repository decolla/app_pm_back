using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa um anexo associado a uma ordem de serviço.
/// </summary>
public class AnexoOrdemModel : IAuditable
{
    /// <summary>
    /// Identificador único do anexo.
    /// <example>123</example>
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdAnexoOrdem { get; set; }

    /// <summary>
    /// Nome do arquivo original com extensão.
    /// <example>relatorio_manutencao.pdf</example>
    /// </summary>
    [Required(ErrorMessage = "O nome do arquivo é obrigatório.")]
    [MaxLength(250, ErrorMessage = "O nome do arquivo não pode exceder 250 caracteres.")]
    public required string Nome { get; set; }

    /// <summary>
    /// Tipo MIME do arquivo (ex: image/jpeg, application/pdf).
    /// <example>application/pdf</example>
    /// </summary>
    [Required(ErrorMessage = "O tipo do arquivo é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O tipo do arquivo não pode exceder 100 caracteres.")]
    public required string TipoArquivo { get; set; }

    /// <summary>
    /// Categoria ou classificação do anexo para fins de organização.
    /// <example>Evidência Fotográfica</example>
    /// </summary>
    [MaxLength(100, ErrorMessage = "O tipo de anexo não pode exceder 100 caracteres.")]
    public string? TipoAnexo { get; set; }

    /// <summary>
    /// Conteúdo binário do arquivo armazenado no banco de dados.
    /// <example>[0x25, 0x50, 0x44, 0x46, ...]</example>
    /// </summary>
    public byte[]? Dados { get; set; }

    /// <summary>
    /// Chave estrangeira para a ordem de serviço relacionada.
    /// <example>5001</example>
    /// </summary>
    [Required(ErrorMessage = "O ID da ordem é obrigatório.")]
    public long? IdOrdem { get; set; }

    /// <summary>
    /// Propriedade de navegação para a entidade Ordem relacionada.
    /// Permite o carregamento (Eager ou Lazy Loading) dos dados da ordem.
    /// </summary>
    [ForeignKey("IdOrdem")]
    public virtual OrdemModel? Ordem { get; set; }
}
