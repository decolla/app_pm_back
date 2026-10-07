using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa a entidade de Usuário no sistema.
/// Exemplo: IdUsuario: 1, Nome: "João Silva".
/// </summary>
public class UsuarioModel
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdUsuario { get; set; }
    
    /// <summary>
    /// Armazena o UUID do Keycloak/Portal para integração de autenticação.
    /// </summary>
    [MaxLength(50)]
    public string? PortalId { get; set; }

    /// <summary>
    /// Nome completo do usuário.
    /// Exemplo: "Ana Costa".
    /// </summary>
    [MaxLength(150)]
    public string? Nome { get; set; }

    /// <summary>
    /// Cadastro de Pessoa Física.
    /// Exemplo: "123.456.789-00".
    /// </summary>
    [MaxLength(20)]
    public string? CPF { get; set; }

    /// <summary>
    /// Telefone de contato do usuário.
    /// </summary>
    public string? Telefone { get; set; }

    /// <summary>
    /// Nível de permissão do usuário no sistema.
    /// </summary>
    public NivelPermissao? NivelPermissao { get; set; } = Domain.NivelPermissao.Usuario;

    /// <summary>
    /// Lista de escopos aos quais o usuário tem acesso.
    /// </summary>
    public List<string> Escopos { get; set; } = new List<string>();
    
    /// <summary>
    /// Lista de favoritos do usuário, representando telas favoritadas 
    /// </summary>
    public List<string> Favoritos { get; set; } = new List<string>();

    /// <summary>
    /// Coleção de notas associadas ao usuário.
    /// </summary>
    public virtual ICollection<NotaModel> Notas { get; set; } = new List<NotaModel>();
    
    /// <summary>
    /// Coleção de ordens que o usuário é responsável
    /// </summary>
    public virtual ICollection<OrdemModel> Ordens { get; set; } = new List<OrdemModel>();

    /// <summary>
    /// Coleção de dados de renovação vinculados.
    /// </summary>
    public virtual ICollection<DadosRenovacaoModel> DadosRenovacao { get; set; } = new List<DadosRenovacaoModel>();

    /// <summary>
    /// Almoxarifados sob responsabilidade ou acesso do usuário.
    /// </summary>
    public virtual ICollection<AlmoxarifadoModel> Almoxarifados { get; set; } = new List<AlmoxarifadoModel>();

    /// <summary>
    /// Registros de GPM associados.
    /// </summary>
    public virtual ICollection<GpmModel> Gpms { get; set; } = new List<GpmModel>();

    /// <summary>
    /// Registros de CTM associados.
    /// </summary>
    public virtual ICollection<CtmModel> Ctms { get; set; } = new List<CtmModel>();
}
