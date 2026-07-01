using System.ComponentModel.DataAnnotations;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

public class ConfiguracaoGlobalDto
{
    [EnumDataType(typeof(OrigemFichasConfiabilidade))]
    public OrigemFichasConfiabilidade FichasConfiabilidade { get; set; }

    [EnumDataType(typeof(ModoClassificacaoAutomatica))]
    public ModoClassificacaoAutomatica ClassificacaoAutomatica { get; set; }
    
    [EnumDataType(typeof(CriterioDataRenovacao))]
    public CriterioDataRenovacao CriterioDataRenovacao { get; set; }
    
    [EnumDataType(typeof(CriterioConfiabilidadeRenovacao))]
    public CriterioConfiabilidadeRenovacao CriterioConfiabilidadeRenovacao { get; set; }
}