using System.ComponentModel.DataAnnotations;

namespace app_pm_back.Models.Domain;

public class TipoManutencaoModel
{
    [Key]
    public int IdTipoManutencao { get; set; }

    [MaxLength(100)]
    public required string Categoria { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    [MaxLength(100)]
    public required string TipoAtivo { get; set; }

    public double Confiabilidade { get; set; }

    public bool Habilitada { get; set; }

    public virtual ICollection<TipoEstrategiaModel> TiposEstrategia { get; set; } = new List<TipoEstrategiaModel>();
    public virtual ICollection<OperacaoModel> Operacoes { get; set; } = new List<OperacaoModel>();
    public virtual ICollection<TipoPlanoModel> TiposPlano { get; set; } = new List<TipoPlanoModel>();
}
