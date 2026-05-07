using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de matriz LIBRA Comgas.
/// </summary>
public class MatrizLibraComgasDto
{
    [SheetColumn("Codigo libra")]
    public string? CodigoLibra { get; set; }

    [SheetColumn("Data criacao")]
    public DateTime? DataCriacao { get; set; }

    [SheetColumn("TAG equipamento")]
    public string? EquipamentoTag { get; set; }

    [SheetColumn("Validade")]
    public DateTime? ValidadeAte { get; set; }
}

#pragma warning restore CS1591

