using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.CongasModel;

public class TipoManutencaoModel
{
    /// <summary>
    /// Id do tipo de manutencao.
    /// </summary>
    [Key]
    public int IdTipoManutencao { get; set; } 
    
    /// <summary>
    /// Nome do tipo de manutencao.
    /// </summary>
    public string NomeTipoManutencao { get; set; }
    
    /// <summary>
    /// Se o tipo está habilitado.
    /// </summary>
    public bool Habilitada { get; set; } = true;
    
    /// <summary>
    /// Confiabilidade do tipo de manutencao em porcentagem.
    /// </summary>
    [Range(0, 100)]
    public int Confiabilidade { get; set; }
    
    /// <summary>
    /// Tipo de ativo.
    /// </summary>
    /// <example>ERP</example>
    public string TipoAtivo { get; set; } 
    
    //MUDAR DPS PARA UMA TABELA
    /// <summary>
    /// Guarda ferramentas e kits específicos dos equipamentos 
    /// </summary>
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> Ferramentas { get; set; } = new Dictionary<string, string>();

}