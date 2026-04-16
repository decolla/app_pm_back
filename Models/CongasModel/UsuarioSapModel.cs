using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Usuario do SAP responsavel por criar e atualizar registros de manutencao.
/// </summary>
public class UsuarioSapModel
{
    /// <summary>
    /// Identificador interno do usuario.
    /// </summary>
    [Key]
    public int IdUsuario { get; set; }

    /// <summary>
    /// Nome de exibicao do usuario.
    /// </summary>
    [MaxLength(120)]
    public required string Nome { get; set; }

    /// <summary>
    /// Credencial do usuario no SAP.
    /// </summary>
    [MaxLength(256)]
    public required string Senha { get; set; }

    /// <summary>
    /// Mandante SAP.
    /// </summary>
    [MaxLength(10)]
    public required string Mandante { get; set; }

    /// <summary>
    /// Idioma padrao do usuario.
    /// </summary>
    [MaxLength(10)]
    public string? Idioma { get; set; }

    /// <summary>
    /// Notas criadas por este usuario.
    /// </summary>
    public virtual ICollection<NotaManutencaoModel> NotasCriadas { get; set; } = new List<NotaManutencaoModel>();

    /// <summary>
    /// Ordens criadas por este usuario.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> OrdensCriadas { get; set; } = new List<OrdemManutencaoModel>();

    /// <summary>
    /// Ordens modificadas por este usuario.
    /// </summary>
    public virtual ICollection<OrdemManutencaoModel> OrdensModificadas { get; set; } = new List<OrdemManutencaoModel>();
}

