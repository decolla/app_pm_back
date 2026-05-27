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
    public long? IdOrdem { get; set; }

    [SheetColumn("Operação")] 
    public string? Nome { get; set; }

    [SheetColumn("Duração")]
    public double? Duracao { get; set; }

    [SheetColumn("Nº profissionais")]
    public int? NdeProfissionais { get; set; }

    [SheetColumn("Texto breve")]
    public string? Descricao { get; set; }

    [SheetColumn("Data início")]
    public DateTime? InicioEsperado { get; set; }

    [SheetColumn("Data fim")]
    public DateTime? FimEsperado { get; set; }

    // Relacionamentos opcionais
    public int? IdCTM { get; set; }

    public int? IdTipoManutencao { get; set; }
}

#pragma warning restore CS1591
