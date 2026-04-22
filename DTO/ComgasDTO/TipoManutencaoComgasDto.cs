using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de manutencao Comgas.
/// </summary>
public class TipoManutencaoComgasDto
{
    [SheetColumn("Id Tipo Manutenção")]
    public int IdTipoManutencao { get; set; }

    [SheetColumn("Tipo do tipo")]
    public int TipoTipo { get; set; }

    [SheetColumn("Nome tipo manutenção")]
    public string? NomeTipoManutencao { get; set; }

    [SheetColumn("Habilitada")]
    public bool Habilitada { get; set; }

    [SheetColumn("Confiabilidade")]
    public int Confiabilidade { get; set; }

    [SheetColumn("Tipo ativo")]
    public string? TipoAtivo { get; set; }

    [SheetColumn("Id Tipo Estratégia")]
    public int? TipoEstrategiaId { get; set; }

    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoId { get; set; }
    
    public List<FerramentaTipoDto> Ferramentas { get; set; } = new();
    public List<ItemRenovacaoTipoDto> ItensRenovacao { get; set; } = new();
}

public class FerramentaTipoDto
{
    public int? IdFerramenta { get; set; }
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public int QuantidadeDisp { get; set; }
    public bool Checkbox { get; set; }
}

public class ItemRenovacaoTipoDto
{
    public int? IdItemRenovacaoTipo { get; set; }
    public required string CodigoItem { get; set; }
    public string? Observacoes { get; set; }
    public string? Template1 { get; set; }
    public string? Template2 { get; set; }
}

#pragma warning restore CS1591
