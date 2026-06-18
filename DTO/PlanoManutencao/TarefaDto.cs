using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tarefa vinculada ao tipo de manutencao.
/// </summary>
public class TarefaDto
{
    [SheetColumn("Id Tarefa")] public long IdOperacao { get; set; }

    [SheetColumn("Nome Tarefa")] public string? Nome { get; set; }

    [SheetColumn("Descrição Tarefa")] public string? Descricao { get; set; }

    [SheetColumn("Duração")] public double Duracao { get; set; }

    [SheetColumn("Nº profissionais")] public int NdeProfissionais { get; set; }

    [SheetColumn("Data início")] public DateTime? InicioEsperado { get; set; }

    [SheetColumn("Data fim")] public DateTime? FimEsperado { get; set; }

    [SheetColumn("Id CTM")] public long? IdCTM { get; set; }

    [SheetColumn("Id Tipo Manutenção")] public long? IdTipoManutencao { get; set; }

    [SheetColumn("Id Ordem")] public long? IdOrdem { get; set; }
}

#pragma warning restore CS1591