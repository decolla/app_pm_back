using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa um anexo associado a uma nota no sistema.
/// </summary>
public class AnexoNotaModel
{
    /// <summary>
    /// Identificador único do anexo.
    /// </summary>
    [Key]
    public int IdAnexoNota { get; set; }

    /// <summary>
    /// Nome do arquivo anexo.
    /// </summary>
    /// <example>relatorio_financeiro.pdf</example>
    [Required(ErrorMessage = "O nome do arquivo é obrigatório.")]
    [MaxLength(250, ErrorMessage = "O nome do arquivo não pode exceder 250 caracteres.")]
    public required string Nome { get; set; }

    /// <summary>
    /// Tipo MIME do arquivo (ex: application/pdf, image/png).
    /// </summary>
    /// <example>application/pdf</example>
    [Required(ErrorMessage = "O tipo do arquivo é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O tipo do arquivo não pode exceder 100 caracteres.")]
    public required string TipoArquivo { get; set; }

    /// <summary>
    /// Conteúdo binário do arquivo.
    /// </summary>
    public byte[]? Dados { get; set; }

    /// <summary>
    /// Chave estrangeira para a nota relacionada.
    /// </summary>
    public long IdNota { get; set; }

    /// <summary>
    /// Propriedade de navegação para a nota pai.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
}
