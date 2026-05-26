using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

public class OrdemModel : IClassifiable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdOrdem { get; set; }

    public DateTime DataCriacao { get; set; }

    [MaxLength(150)]
    public string? StatusOrdem { get; set; }

    [MaxLength(100)]
    public string? Criticidade { get; set; }

    [MaxLength(100)]
    public string? Classificacao { get; set; }

    [MaxLength(100)]
    public string? MetodoClassificacao { get; set; }

    [MaxLength(150)]
    public string? ClassificadoPor { get; set; }

    public DateTime? DataClassificacao { get; set; }

    public DateTime? InicioPlanejado { get; set; }

    public DateTime? FimPlanejado { get; set; }

    public DateTime? InicioReal { get; set; }

    public DateTime? FimReal { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    [MaxLength(150)]
    public string? CriadaPorUsuario { get; set; }

    [MaxLength(150)]
    public string? ModificadaPorUsuario { get; set; }

    public DateTime? DataModificacao { get; set; }

    public int? IdAtivo { get; set; } // Stores Ativo ID only

    public long? IdNota { get; set; }

    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }

    public virtual ICollection<AnexoOrdemModel> Anexos { get; set; } = new List<AnexoOrdemModel>();
    public virtual ICollection<OperacaoModel> Operacoes { get; set; } = new List<OperacaoModel>();
}
