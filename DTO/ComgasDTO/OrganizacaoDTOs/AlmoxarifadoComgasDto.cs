using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de almoxarifado Comgas.
/// </summary>
public class AlmoxarifadoComgasDto
{
    [SheetColumn("Id Almoxarifado")]
    public int IdAlmoxarifado { get; set; }

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("Organizacao")]
    public string? Organizacao { get; set; }

    [SheetColumn("Localizacao")]
    public int? LocalizacaoId { get; set; }

    [SheetColumn("Status")]
    public string? Status { get; set; }

    [SheetColumn("Descricao")]
    public string? Descricao { get; set; }
}

#pragma warning restore CS1591

