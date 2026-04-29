using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Funcionario cadastrado para vinculo com centros, setores e almoxarifados.
/// </summary>
public class FuncionarioModel
{
    /// <summary>
    /// Identificador interno do funcionario.
    /// </summary>
    [Key]
    public int IdFuncionario { get; set; }

    /// <summary>
    /// Codigo do funcionario (ex.: matricula SAP).
    /// </summary>
    [MaxLength(30)]
    public string? CodigoFuncionario { get; set; }

    /// <summary>
    /// Nome completo do funcionario.
    /// </summary>
    [MaxLength(150)]
    public required string Nome { get; set; }

    /// <summary>
    /// CPF do funcionario.
    /// </summary>
    [MaxLength(14)]
    public required string Cpf { get; set; }

    /// <summary>
    /// Funcao exercida pelo funcionario.
    /// </summary>
    [MaxLength(80)]
    public string? Funcao { get; set; }

    /// <summary>
    /// Organizacao vinculada ao funcionario.
    /// </summary>
    [MaxLength(80)]
    public string? Organizacao { get; set; }

    /// <summary>
    /// Perfil de permissao do funcionario no sistema.
    /// </summary>
    public PermissaoFuncionario Permissao { get; set; }

    /// <summary>
    /// Centros de trabalho vinculados.
    /// </summary>
    public virtual ICollection<CentroTrabalhoModel> CentrosTrabalho { get; set; } = new List<CentroTrabalhoModel>();

    /// <summary>
    /// Setores de vendas vinculados.
    /// </summary>
    public virtual ICollection<SetorVendasModel> SetoresVendas { get; set; } = new List<SetorVendasModel>();

    /// <summary>
    /// Almoxarifados vinculados.
    /// </summary>
    public virtual ICollection<AlmoxarifadoModel> Almoxarifados { get; set; } = new List<AlmoxarifadoModel>();
}

/// <summary>
/// Perfis de permissao cadastrados para funcionarios.
/// </summary>
public enum PermissaoFuncionario
{
    Todos = 0,
    Comum = 1,
    Admin = 2,
    Gpm = 3,
    Ctm = 4
}

