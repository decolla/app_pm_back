using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;


public class DadosRenovacaoDto
{
    /// <summary>
    /// Identificador único do registro de renovação.
    /// </summary>
    public int IdDados { get; set; }

    /// <summary>
    /// Data e hora da última renovação realizada.
    /// </summary>
    public DateTime UltimaRenovacao { get; set; }

    /// <summary>
    /// Origem da última renovação (nota, ordem, usuario).
    /// </summary>
    public OrigemUR OrigemUR { get; set; }
    
    /// <summary>
    /// Se a renovação é corretiva/preventiva e de usuario/catboost
    /// </summary>
    
    public TipoDados TipoDados { get; set; }

    /// <summary>
    /// Índice de confiabilidade calculado para o ativo (0 a 1 ou percentual).
    /// </summary>
    public double? Confiabilidade { get; set; }

    /// <summary>
    /// Identificador do Ativo relacionado.
    /// </summary>
    public ComponentType? IdAtivo { get; set; }
    
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a nota a um local específico de instalação.
    /// </summary>
    public string? IdLi { get; set; }

    /// <summary>
    /// Identificador da Nota relacionada.
    /// </summary>
    public long? IdNota { get; set; }

    /// <summary>
    /// Identificador da Ordem de Serviço relacionada.
    /// </summary>
    public long? IdOrdem { get; set; }

    /// <summary>
    /// Identificador do Usuário que processou a renovação.
    /// </summary>
    public int? IdUsuario { get; set; }
}