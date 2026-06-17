using System.ComponentModel.DataAnnotations;

namespace app_pm_back.DTO;

public class ReliabilityDTO
{
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a nota a um local específico de instalação.
    /// </summary>
    [MaxLength(100)]
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