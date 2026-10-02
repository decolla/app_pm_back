using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa uma operação técnica dentro de uma ordem de manutenção.
/// Exemplo: "Troca de óleo", "Inspeção visual".
/// </summary>
public class OperacaoModel : IAuditable
{
    /// <summary>
    /// Identificador único da operação.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdOperacao { get; set; }

    /// <summary>
    /// Nome ou título da operação.
    /// Exemplo: "Lubrificação de mancais".
    /// </summary>
    [MaxLength(150)]
    public string? Nome { get; set; }

    /// <summary>
    /// Detalhamento técnico do que deve ser executado.
    /// </summary>
    [MaxLength(7000)]
    public string? Descricao { get; set; }
    
    /// <summary>
    /// Indica se a operação foi concluída.
    /// </summary>
    public bool Concluida { get; set; }

    /// <summary>
    /// Duração estimada para a conclusão em horas.
    /// Exemplo: 1.5
    /// </summary>
    public double? Duracao { get; set; }

    /// <summary>
    /// Quantidade de profissionais necessários para a tarefa.
    /// </summary>
    public int? NdeProfissionais { get; set; }
    
    /// <summary>
    /// Custo da tarefa da manutenção
    /// </summary>
    public float? CustosAdicionais { get; set; }

    /// <summary>
    /// Data e hora prevista para o início.
    /// </summary>
    public DateTime? InicioEsperado { get; set; }

    /// <summary>
    /// Data e hora prevista para a conclusão.
    /// </summary>
    public DateTime? FimEsperado { get; set; }

    /// <summary>
    /// Chave estrangeira para o Centro de Trabalho Móvel (CTM).
    /// </summary>
    public long? IdCTM { get; set; }

    /// <summary>
    /// Chave estrangeira para a categoria de manutenção.
    /// </summary>
    public long? IdTipoManutencao { get; set; }

    /// <summary>
    /// Chave estrangeira para a Ordem de Manutenção pai.
    /// </summary>
    public long? IdOrdem { get; set; }

    /// <summary>
    /// Objeto de navegação para o CTM associado.
    /// </summary>
    [ForeignKey(nameof(IdCTM))]
    public virtual CtmModel? Ctm { get; set; }

    /// <summary>
    /// Objeto de navegação para o Tipo de Manutenção associado.
    /// </summary>
    [ForeignKey(nameof(IdTipoManutencao))]
    public virtual TipoManutencaoModel? TipoManutencao { get; set; }

    /// <summary>
    /// Objeto de navegação para a Ordem de Manutenção associada.
    /// </summary>
    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemModel? Ordem { get; set; }

    /// <summary>
    /// Lista de ferramentas vinculadas a esta operação.
    /// </summary>
    public virtual ICollection<FerramentaModel> Ferramentas { get; set; } = new List<FerramentaModel>();
}
