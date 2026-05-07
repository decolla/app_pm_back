using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de plano de manutencao Comgas.
/// </summary>
public class PlanoManutencaoComgasDto
{
    [SheetColumn("Plano manut.")]
    public long CodigoPlano { get; set; }

    [SheetColumn("Nome plano")]
    public string? NomePlano { get; set; }
    
    [SheetColumn("Ativo")]
    public string? NomeAtivo { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Prioridade")]
    public int? Prioridade { get; set; }

    [SheetColumn("Gerar automaticamente")]
    public int? GerarAutomaticamente { get; set; }

    [SheetColumn("Dt.inicio")]
    public DateTime? DataInicio { get; set; }

    [SheetColumn("Data conclusão")]
    public DateTime? DataConclusao { get; set; }

    [SheetColumn("Antecedência")]
    public int Antencedencia { get; set; }

    [SheetColumn("Última ordem")]
    public long? UltimaOrdem { get; set; }

    [SheetColumn("Id Estratégia")]
    public int? EstrategiaId { get; set; }
}

#pragma warning restore CS1591
