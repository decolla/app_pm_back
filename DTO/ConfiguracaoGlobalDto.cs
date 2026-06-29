using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

public class ConfiguracaoGlobalDto
{
    public OrigemFichasConfiabilidade FichasConfiabilidade { get; set; }

    public ModoClassificacaoAutomatica ClassificacaoAutomatica { get; set; }
    
    public CriterioDataRenovacao CriterioDataRenovacao { get; set; }
    
    public CriterioConfiabilidadeRenovacao CriterioConfiabilidadeRenovacao { get; set; }
}