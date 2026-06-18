using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO do cadastro de tipo de manutencao CEP.
/// </summary>
public class TipoManutencaoCepDto
{
    [SheetColumn("CEP")] public long IdTipoCep { get; set; }
    [SheetColumn("Tipo Ativo")] public string? TipoAtivo { get; set; }

    [SheetColumn("Confiabilidade")] public double? Confiabilidade { get; set; }

    [SheetColumn("Habilitada")] public bool? Habilitada { get; set; }

    [SheetColumn("Campo indefinido")] public string? CampoIndefinido { get; set; }

    [SheetColumn("Ações Falha")] public List<AcoesFalha>? AcoesFalha { get; set; } = new();
}

#pragma warning restore CS1591