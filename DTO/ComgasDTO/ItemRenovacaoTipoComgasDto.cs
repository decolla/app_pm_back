using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de item de renovacao vinculado ao tipo de manutencao.
/// </summary>
public class ItemRenovacaoTipoComgasDto
{
    [SheetColumn("Id Item Renovação Tipo")]
    public int IdItemRenovacaoTipo { get; set; }

    [SheetColumn("Código Item")]
    public string? CodigoItem { get; set; }

    [SheetColumn("Observações")]
    public string? Observacoes { get; set; }

    [SheetColumn("Template 1")]
    public string? Template1 { get; set; }

    [SheetColumn("Template 2")]
    public string? Template2 { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int TipoManutencaoId { get; set; }
}

#pragma warning restore CS1591

