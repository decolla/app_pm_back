using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de setor de vendas Comgas.
/// </summary>
public class SetorVendasComgasDto
{
    [SheetColumn("Id Setor")]
    public int IdSetor { get; set; }

    [SheetColumn("Nome")]
    public string? Nome { get; set; }

    [SheetColumn("Organizacao")]
    public string? Organizacao { get; set; }

    [SheetColumn("Localizacao")]
    public int? LocalizacaoId { get; set; }

    [SheetColumn("Descricao")]
    public string? Descricao { get; set; }
}

#pragma warning restore CS1591

