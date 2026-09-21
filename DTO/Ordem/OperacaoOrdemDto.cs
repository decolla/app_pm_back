using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de operacao de ordem de manutencao.
/// </summary>
public class OperacaoOrdemDto
{
    public long IdOperacao { get; set; }

    public long? IdOrdem { get; set; }

    [SheetColumn("Nº objeto")] public string? Nome { get; set; }

    public double? Duracao { get; set; }

    public int? NdeProfissionais { get; set; }

    [SheetColumn("TEXTO OPERAÇÃO LONGO (PROCV)")] public string? Descricao { get; set; }

    [SheetColumn("Início prog.")] public DateTime? InicioEsperado { get; set; }

    [SheetColumn("Fim programado")] public DateTime? FimEsperado { get; set; }
    
    [SheetColumn("Ferramentas")]
    public List<FerramentaTipoDto>? Ferramentas { get; set; } = new List<FerramentaTipoDto>();

    public long? IdCTM { get; set; }

    public long? IdTipoManutencao { get; set; }
}

#pragma warning restore CS1591