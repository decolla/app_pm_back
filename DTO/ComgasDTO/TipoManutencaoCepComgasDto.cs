using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO do cadastro de tipo de manutencao CEP.
/// </summary>
public class TipoManutencaoCepComgasDto
{
    [SheetColumn("Id Tipo Manutenção CEP")]
    public int IdTipoManutencaoCep { get; set; }

    [SheetColumn("Nome Tipo")]
    public string? NomeTipo { get; set; }

    [SheetColumn("Tipo Ativo")]
    public string? TipoAtivo { get; set; }

    [SheetColumn("Confiabilidade")]
    public int Confiabilidade { get; set; }

    [SheetColumn("Habilitada")]
    public bool Habilitada { get; set; }

    [SheetColumn("Níveis")]
    public Dictionary<string, string>? Niveis { get; set; }

    [SheetColumn("Ações Falha")]
    public List<int> AcoesFalha { get; set; } = new();

    [SheetColumn("Plano Manutenção Código")]
    public long? PlanoManutencaoCodigo { get; set; }
}

#pragma warning restore CS1591

