using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

/// <summary>
/// DTO de importacao para usuarios SAP .
/// </summary>
public class UsuarioDto
{
    /// <summary>
    /// Identificador do usuario no SAP interno.
    /// </summary>
    [SheetColumn("ID")]
    public long IdUsuario { get; set; }

    /// <summary>
    /// Nome do usuario.
    /// </summary>
    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    /// <summary>
    /// CPF do usuario.
    /// </summary>
    [SheetColumn("CPF")]
    public string? CPF { get; set; }
    
    [SheetColumn("Telefone")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Permissoes (lista serializada ou descricao textual).
    /// </summary>
    [SheetColumn("Permissoes")]
    public string? Permissoes { get; set; }

    /// <summary>
    /// Nível de permissão do usuário no sistema.
    /// </summary>
    public NivelPermissao? NivelPermissao { get; set; } = Models.Domain.NivelPermissao.Usuario;

    /// <summary>
    /// Lista de escopos aos quais o usuário tem acesso.
    /// </summary>
    [SheetColumn("Id Escopo")]
    public List<long> Escopos { get; set; } = new List<long>();
    
    /// <summary>
    /// Lista de favoritos do usuário.
    /// </summary>
    [SheetColumn("Favoritos")]
    public List<string> Favoritos { get; set; } = new List<string>();
}
