using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class PlanoModel
{
    [Key]
    public long IdPlano { get; set; }

    [MaxLength(150)]
    public required string Nome { get; set; }

    public Prioridade? Prioridade { get; set; }

    public DateTime? DataInicios { get; set; }

    public GerarAutomaticamente? GerarAutomaticamente { get; set; }

    public int Antecedencia { get; set; }

    public DateTime? DataConclusao { get; set; }

    [MaxLength(150)]
    public string? NomeAtivo { get; set; }

    public int IdAtivo { get; set; } // Stores Ativo ID only

    public int IdEstrategia { get; set; }

    [ForeignKey(nameof(IdEstrategia))]
    public virtual EstrategiaModel? Estrategia { get; set; }
}
