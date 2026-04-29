using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Setor de vendas cadastrado por organizacao.
/// </summary>
public class SetorVendasModel
{
    /// <summary>
    /// Identificador interno do setor.
    /// </summary>
    [Key]
    public int IdSetor { get; set; }

    /// <summary>
    /// Nome do setor.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// Organizacao responsavel pelo setor.
    /// </summary>
    [MaxLength(80)]
    public required string Organizacao { get; set; }

    /// <summary>
    /// Localizacao vinculada ao setor.
    /// </summary>
    public int? LocalizacaoId { get; set; }

    /// <summary>
    /// Descricao do setor.
    /// </summary>
    [MaxLength(300)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Localizacao associada.
    /// </summary>
    [ForeignKey(nameof(LocalizacaoId))]
    public virtual LocalizacaoModel? Localizacao { get; set; }

    /// <summary>
    /// Funcionarios vinculados ao setor.
    /// </summary>
    public virtual ICollection<FuncionarioModel> Funcionarios { get; set; } = new List<FuncionarioModel>();
}

