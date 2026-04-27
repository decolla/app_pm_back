using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ferramenta vinculada ao tipo de manutencao.
/// </summary>
public class FerramentaTipoComgasDto
{
    [SheetColumn("Id Ferramenta")]
    public int IdFerramenta { get; set; }

    [SheetColumn("Selecionada")]
    public bool Checkbox { get; set; }

    [SheetColumn("Nome Ferramenta")]
    public string? Nome { get; set; }

    [SheetColumn("Descrição Ferramenta")]
    public string? Descricao { get; set; }

    [SheetColumn("Quantidade Disponível")]
    public int QuantidadeDisp { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int TipoManutencaoId { get; set; }
}

#pragma warning restore CS1591

