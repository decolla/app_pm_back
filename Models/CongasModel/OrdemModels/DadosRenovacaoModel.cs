using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class DadosRenovacaoModel
{
    [Key]
    public int IdDadosRenovacao { get; set; }
    
    public DateTime? UltimaRenovacao { get; set; }

    public TipoUr? TipoUr { get; set; } 

    [Range(0,100)] public int Confiabilidade { get; set; } = 100;
    
    public string? Ativo { get; set; }
    
    public int? IdOrdem { get; set; }
    
    public int? IdNota { get; set; }
    
    public int? IdUsuario { get; set; }
    
    [ForeignKey(nameof(IdOrdem))]
    public virtual OrdemManutencaoModel? OrdemManutencao { get; set; }
    
    [ForeignKey(nameof(IdNota))]
    public virtual NotaManutencaoModel? NotaManutencao { get; set; }
    
    [ForeignKey(nameof(IdUsuario))]
    public virtual UsuarioSapModel? Usuario { get; set; }
}

public enum TipoUr
{
    Nota = 0,
    Ordem = 1,
    Usuario = 2
}