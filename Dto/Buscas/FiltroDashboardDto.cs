using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.Buscas;

public class FiltroDashboardDto
{
    public ComponentType? Ativo { get; set; }
    
    public int? Ultimos { get; set; }

    public UnidadeTempo UnidadeTempo { get; set; } = UnidadeTempo.Meses;
    
    // public long? NumeroRegistro { get; set; }
    
    public string? IdLi { get; set; }
    
}