using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao para usuarios SAP da Comgas.
/// </summary>
public class UsuarioSapComgasDto
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
    /// Senha do usuario.
    /// </summary>
    [SheetColumn("Senha")]
    public string? Senha { get; set; }

    /// <summary>
    /// Mandante SAP.
    /// </summary>
    [SheetColumn("Mandante")]
    public string? Mandante { get; set; }

    /// <summary>
    /// Idioma padrao.
    /// </summary>
    [SheetColumn("Idioma")]
    public string? Idioma { get; set; }
}

#pragma warning restore CS1591
