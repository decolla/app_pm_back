using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tarefa vinculada ao tipo de manutencao.
/// </summary>
public class TarefaComgasDto
{
    [SheetColumn("Id Tarefa")]
    public int IdTarefa { get; set; }

    [SheetColumn("Nome Tarefa")]
    public string? Nome { get; set; }

    [SheetColumn("Descrição Tarefa")]
    public string? Descricao { get; set; }

    [SheetColumn("Duração")]
    public int Duracao { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int TipoManutencaoId { get; set; }
}

#pragma warning restore CS1591

