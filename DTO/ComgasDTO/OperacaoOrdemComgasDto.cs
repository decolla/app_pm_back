using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de operacao de ordem de manutencao Comgas.
/// </summary>
public class OperacaoOrdemComgasDto
{
    [SheetColumn("Id Operação")]
    public int IdOperacao { get; set; }

    [SheetColumn("Ordem")]
    public long OrdemId { get; set; }

    [SheetColumn("Operação")]
    public string? NomeOperacao { get; set; }

    [SheetColumn("Tipo ativid.PM")]
    public string? TipoAtividade { get; set; }

    [SheetColumn("Duração")]
    public decimal? DuracaoHoras { get; set; }

    [SheetColumn("Nº profissionais")]
    public int? NumeroProfissionais { get; set; }

    [SheetColumn("Texto breve")]
    public string? TextoBreve { get; set; }

    [SheetColumn("Data início")]
    public DateTime? DataInicio { get; set; }

    [SheetColumn("Data fim")]
    public DateTime? DataFim { get; set; }
}

#pragma warning restore CS1591
