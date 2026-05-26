using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

public class NotaModel : IClassifiable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdNota { get; set; }

    public long NumeroNota { get; set; }

    public DateTime DataNota { get; set; }

    [MaxLength(200)]
    public string? ReferenciaLocalizacaoAtivo { get; set; }

    [MaxLength(120)]
    public string? Finalidade { get; set; }

    public int? Prioridade { get; set; }

    public DateTime? FimDesejado { get; set; }

    public DateTime? DataAvaria { get; set; }

    public TimeSpan? HoraAvaria { get; set; }

    [MaxLength(50)]
    public string? StatusNota { get; set; }

    [MaxLength(50)]
    public string? EquipamentoDisponivel { get; set; }

    [MaxLength(1000)]
    public string? AnomaliaObservada { get; set; }

    [MaxLength(2000)]
    public string? ObservacoesComplementares { get; set; }

    [MaxLength(100)]
    public string? Classificacao { get; set; }

    [MaxLength(100)]
    public string? MetodoClassificacao { get; set; }

    [MaxLength(150)]
    public string? ClassificadoPor { get; set; }

    public DateTime? DataClassificacao { get; set; }

    [MaxLength(150)]
    public string? CriadaPorUsuario { get; set; }

    [MaxLength(150)]
    public string? ModificadaPorUsuario { get; set; }

    public DateTime? DataModificacao { get; set; }

    public long? IdPlano { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdAtivo { get; set; } // Stores Ativo ID only

    [ForeignKey(nameof(IdPlano))]
    public virtual PlanoModel? Plano { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual UsuarioModel? Usuario { get; set; }

    public virtual ICollection<VariavelNotaModel> Variaveis { get; set; } = new List<VariavelNotaModel>();
    public virtual ICollection<ItemNotaModel> Itens { get; set; } = new List<ItemNotaModel>();
    public virtual ICollection<AnexoNotaModel> Anexos { get; set; } = new List<AnexoNotaModel>();
}
