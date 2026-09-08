using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa as configurações globais do sistema. Esta tabela possuirá apenas o ID = 1.
/// </summary>
public class ConfiguracaoGlobalModel
{
    [System.ComponentModel.DataAnnotations.Key]
    public int Id { get; set; } = 1; // travado em 1 para garantir linha única
    
    /// <summary>
    /// Modelo declarado como padrão para a classificação do catboost
    /// </summary>
    [MaxLength(255)]
    public string? ModeloCatboostPadrao {get; set;}

    /// <summary>
    /// Fichas que serão usadas para calcular a confiabilidade.
    /// </summary>
    public OrigemFichasConfiabilidade FichasConfiabilidade { get; set; }
    
    /// <summary>
    /// Métodos para classificar automaticamente as fichas.
    /// </summary>
    public ModoClassificacaoAutomatica ClassificacaoAutomatica { get; set; }
    
    /// <summary>
    /// Critérios que serão utilizados para a renovação baseada na última data
    /// </summary>
    public CriterioDataRenovacao CriterioDataRenovacao { get; set; }
    
    /// <summary>
    /// Critérios para definir a confiabilidade de uma renovação
    /// </summary>
    public CriterioConfiabilidadeRenovacao CriterioConfiabilidadeRenovacao { get; set; }
    
    /// <summary>
    /// Critérios para definir se a ficha deve ser criada automaticamente periodicamente
    /// </summary>
    public CriarFichasPreventivas CriarFichasPreventivas { get; set; }
    
    /// <summary>
    /// Lista de palavras de natureza de causa para o cadastro de corretivas
    /// </summary>
    public List<string> CadastrarNaturezaCausaCorretiva { get; set; } = new List<string>();
    
    /// <summary>
    /// Lista de palavras de natureza de causa alheia
    /// </summary>
    public List<string> CadastrarNaturezaCausaAlheia { get; set; } = new List<string>();
    
    /// <summary>
    /// Data da última modificação da configuração global.
    /// </summary>
    public DateTime DataModificacao { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Usuário que realizou a última modificação na configuração global
    /// </summary>
    [MaxLength(255)]
    public string? ModificadoPorUsuario { get; set; }
}