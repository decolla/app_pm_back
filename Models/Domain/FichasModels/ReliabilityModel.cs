using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_pm_back.Models.Domain;

public class ReliabilityModel
{
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a nota a um local específico de instalação.
    /// </summary>
    [MaxLength(100)]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public string IdLi { get; set; }
    

    /// <summary>
    /// Identificador da Nota relacionada.
    /// </summary>
    public List<long>? IdsNotas { get; set; }

    /// <summary>
    /// Identificador da Ordem de Serviço relacionada.
    /// </summary>
    public List<long>? IdsOrdens { get; set; }
}