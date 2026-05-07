using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

/// <summary>
/// DTO de atividades sugeridas para notas.
/// </summary>
public class AtividadeNotaComgasDto
{
    [SheetColumn("Id Atividade")]
    public int IdAtividade { get; set; }

    [SheetColumn("Nota")]
    public long NotaId { get; set; }

    [SheetColumn("Txt.ativ.")]
    public string? Descricao { get; set; }
}

