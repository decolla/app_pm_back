using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.Buscas;

public class BuscaDiretorioDto
{
    public string SapName { get; set; }
    public string DnsIot { get; set; }
    public string NroFicha { get; set; }
    public ComponentType Componente { get; set; }
}