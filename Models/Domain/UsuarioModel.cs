using System.ComponentModel.DataAnnotations;

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
    public long IdUsuario { get; set; }

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
    /// Lista de permissões serializada ou descrição de acessos.
    /// Exemplo: 7 -> NOTAS | ORDENS | ITENS.
    /// </summary>
    public PermissoesTabela? Permissoes { get; set; } = PermissoesTabela.Nenhuma;

    /// <summary>
    /// Nível de permissão do usuário no sistema.
    /// </summary>
    public NivelPermissao? NivelPermissao { get; set; } = Domain.NivelPermissao.Usuario;

    /// <summary>
    /// Identificador do escopo ao qual o usuário pertence.
    /// </summary>
    public long? IdEscopo { get; set; }

    /// <summary>
    /// Coleção de notas associadas ao usuário.
    /// </summary>
    public virtual ICollection<NotaModel> Notas { get; set; } = new List<NotaModel>();

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
