using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de permissao de trabalho (PT) Comgas.
/// </summary>
public class PermissaoTrabalhoComgasDto
{
    [SheetColumn("Codigo PT")]
    public string? CodigoPt { get; set; }

    [SheetColumn("Data criacao")]
    public DateTime? DataCriacao { get; set; }

    [SheetColumn("Permissoes")]
    public bool? PermissaoConcedida { get; set; }

    [SheetColumn("TAG ordem")]
    public string? TagOrdem { get; set; }

    [SheetColumn("Ordem")]
    public long? OrdemId { get; set; }
}

#pragma warning restore CS1591

