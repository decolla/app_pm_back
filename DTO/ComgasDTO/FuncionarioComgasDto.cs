using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de funcionario Comgas.
/// </summary>
public class FuncionarioComgasDto
{
    [SheetColumn("Id Funcionario")]
    public int IdFuncionario { get; set; }

    [SheetColumn("Codigo funcionario")]
    public string? CodigoFuncionario { get; set; }

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("CPF")]
    public string? Cpf { get; set; }

    [SheetColumn("Funcao")]
    public string? Funcao { get; set; }

    [SheetColumn("Organizacao")]
    public string? Organizacao { get; set; }

    [SheetColumn("Permissao")]
    public PermissaoFuncionario Permissao { get; set; }
}

#pragma warning restore CS1591

