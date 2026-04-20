using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ordem de manutencao Comgas.
/// </summary>
public class OrdemManutencaoComgasDto
{
    [SheetColumn("Ordem")]
    public long IdOrdem { get; set; }

    [SheetColumn("Tipo de ordem")]
    public string? TipoOrdem { get; set; }

    [SheetColumn("Classificação")]
    public string? Classificacao { get; set; }

    [SheetColumn("Método classificação")]
    public string? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")]
    public string? ClassificadoPor { get; set; }

    [SheetColumn("Data classificação")]
    public DateTime? DataClassificacao { get; set; }

    [SheetColumn("Status sistema")]
    public string? StatusOrdem { get; set; }

    [SheetColumn("Nota")]
    public long? NotaRelacionadaId { get; set; }

    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoCodigo { get; set; }

    [SheetColumn("Prioridade")]
    public int? Prioridade { get; set; }

    [SheetColumn("Data de entrada")]
    public DateTime? DataEntrada { get; set; }

    [SheetColumn("Criado por")]
    public int? CriadaPorUsuarioId { get; set; }

    [SheetColumn("Modificado por")]
    public int? ModificadaPorUsuarioId { get; set; }

    [SheetColumn("Custs.estimados")]
    public decimal? CustoEstimado { get; set; }

    [SheetColumn("Custos tot.plan")]
    public decimal? CustoPlanejado { get; set; }

    [SheetColumn("Cust.tot.reais")]
    public decimal? CustoReal { get; set; }
}

#pragma warning restore CS1591
