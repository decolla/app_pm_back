using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tipo de manutencao .
/// </summary>
public class TipoManutencaoDto
{
    [SheetColumn("Id Tipo Manutenção")] public long IdTipoManutencao { get; set; }

    [SheetColumn("Categoria")] public CategoriaTipoManutencao? Categoria { get; set; }

    [SheetColumn("Nome")] public string? Nome { get; set; }

    [SheetColumn("Tipo Ativo")] public string? TipoAtivo { get; set; }

    [SheetColumn("Confiabilidade")] public double? Confiabilidade { get; set; }

    [SheetColumn("Habilitada")] public bool? Habilitada { get; set; }
}

#pragma warning restore CS1591