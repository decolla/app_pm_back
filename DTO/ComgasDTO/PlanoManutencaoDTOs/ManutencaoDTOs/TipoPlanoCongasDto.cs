using app_pm_back.Attributes;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de manutencao Comgas.
/// </summary>
public class TipoPlanoCongasDto
{
    [SheetColumn("Id Tipo Manutenção")]
    public int IdTipoManutencao { get; set; }

    public Visivel Visivel { get; set; } = Visivel.Visivel;

    public CategoriaTipoEstrategia Categoria { get; set; } = CategoriaTipoEstrategia.Manutencao;

    public int Dias { get; set; } = 80;

    public int ToleranciaPositiva { get; set; }

    public int ToleranciaNegativa { get; set; }

    public bool Continua { get; set; }

    [SheetColumn("Id Tipo Estratégia")]
    public int? TipoEstrategiaId { get; set; }

    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoId { get; set; }

    public int? TipoManutencaoCepId { get; set; }

    public int? TipoManutencaoId { get; set; }

    public List<FerramentaTipoDto> Ferramentas { get; set; } = new();

    public List<TarefaComgasDto> ListaTarefas { get; set; } = new();
}

public class FerramentaTipoDto
{
    public int? IdFerramenta { get; set; }
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public int QuantidadeDisp { get; set; }
    public bool Checkbox { get; set; }
}

#pragma warning restore CS1591
