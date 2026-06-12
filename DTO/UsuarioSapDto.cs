using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao para usuarios SAP .
/// </summary>
public class UsuarioSapDto
{
    /// <summary>
    /// Identificador do usuario no SAP interno.
    /// </summary>
    [SheetColumn("ID")]
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nome do usuario.
    /// </summary>
    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    /// <summary>
    /// CPF do usuario.
    /// </summary>
    [SheetColumn("CPF")]
    public string? Cpf { get; set; }

    /// <summary>
    /// Permissoes (lista serializada ou descricao textual).
    /// </summary>
    [SheetColumn("Permissoes")]
    public string? Permissoes { get; set; }

    /// <summary>
    /// Escopo externo (apenas o ID).
    /// </summary>
    [SheetColumn("Id Escopo")]
    public int? IdEscopo { get; set; }
}

#pragma warning restore CS1591