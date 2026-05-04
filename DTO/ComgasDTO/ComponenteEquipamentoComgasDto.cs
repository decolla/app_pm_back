using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de componente de equipamento Comgas.
/// </summary>
public class ComponenteEquipamentoComgasDto
{
    [SheetColumn("Id Componente")]
    public int IdComponente { get; set; }

    [SheetColumn("TAG equipamento")]
    public string? EquipamentoTag { get; set; }

    [SheetColumn("Denominacao equipamento")]
    public string? DenominacaoEquipamento { get; set; }

    [SheetColumn("Codigo componente")]
    public string? CodigoComponente { get; set; }

    [SheetColumn("Nome componente")]
    public string? NomeComponente { get; set; }

    [SheetColumn("Quantidade")]
    public decimal? Quantidade { get; set; }

    [SheetColumn("Unidade medida")]
    public string? UnidadeMedida { get; set; }

    [SheetColumn("Valido desde")]
    public DateTime? ValidoDesde { get; set; }

    [SheetColumn("Valido ate")]
    public DateTime? ValidoAte { get; set; }

    [SheetColumn("Informacao adicional")]
    public string? InformacaoAdicional { get; set; }
}

#pragma warning restore CS1591

