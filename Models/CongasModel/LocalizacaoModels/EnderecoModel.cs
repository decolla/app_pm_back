using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Endereco usado por entidades de localizacao e ativo.
/// </summary>
public class EnderecoModel
{
    /// <summary>
    /// Identificador interno do endereco.
    /// </summary>
    [Key]
    public int IdEndereco { get; set; }

    /// <summary>
    /// Nome da rua.
    /// </summary>
    [MaxLength(180)]
    public required string Rua { get; set; }

    /// <summary>
    /// CEP do endereco.
    /// </summary>
    [MaxLength(20)]
    public string? Cep { get; set; }

    /// <summary>
    /// Cidade do endereco.
    /// </summary>
    [MaxLength(80)]
    public string? Cidade { get; set; }

    /// <summary>
    /// Bairro do endereco.
    /// </summary>
    [MaxLength(80)]
    public string? Bairro { get; set; }

    /// <summary>
    /// Numero do endereco.
    /// </summary>
    [MaxLength(20)]
    public string? Numero { get; set; }

    /// <summary>
    /// Complemento do endereco.
    /// </summary>
    [MaxLength(120)]
    public string? Complemento { get; set; }

    /// <summary>
    /// Estado do endereco.
    /// </summary>
    [MaxLength(80)]
    public string? Estado { get; set; }

    /// <summary>
    /// Pais do endereco.
    /// </summary>
    [MaxLength(80)]
    public string? Pais { get; set; }

    /// <summary>
    /// Telefone de contato principal.
    /// </summary>
    [MaxLength(30)]
    public string? Telefone { get; set; }

    /// <summary>
    /// Localizacoes vinculadas ao endereco.
    /// </summary>
    public virtual ICollection<LocalizacaoModel> Localizacoes { get; set; } = new List<LocalizacaoModel>();
}

