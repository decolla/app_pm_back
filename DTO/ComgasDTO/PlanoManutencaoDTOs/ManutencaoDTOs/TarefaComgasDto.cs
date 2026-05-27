using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tarefa vinculada ao tipo de manutencao.
/// </summary>
public class TarefaComgasDto
{
    [SheetColumn("Id Tarefa")]
    public int IdOperacao { get; set; }

    [SheetColumn("Nome Tarefa")]
    public string? Nome { get; set; }

    [SheetColumn("Descrição Tarefa")]
    public string? Descricao { get; set; }

    [SheetColumn("Duração")]
    public double Duracao { get; set; }

    [SheetColumn("Nº profissionais")]
    public int NdeProfissionais { get; set; }

    [SheetColumn("Data início")]
    public DateTime? InicioEsperado { get; set; }

    [SheetColumn("Data fim")]
    public DateTime? FimEsperado { get; set; }

    [SheetColumn("Id CTM")]
    public int? IdCTM { get; set; }

    [SheetColumn("Id Tipo Manutenção")]
    public int? IdTipoManutencao { get; set; }

    [SheetColumn("Id Ordem")]
    public long? IdOrdem { get; set; }
}

#pragma warning restore CS1591

