using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de tarefa vinculada ao tipo de manutencao.
/// </summary>
public class TarefaDto
{
    [SheetColumn("Id Tarefa")] public long IdTarefa { get; set; }

    [SheetColumn("Nome Tarefa")] public string? Nome { get; set; }

    [SheetColumn("Descrição Tarefa")] public string? Descricao { get; set; }

    [SheetColumn("Duração")] public double Duracao { get; set; }

    [SheetColumn("Nº profissionais")] public int NdeProfissionais { get; set; }
    
    [SheetColumn("Custos Adicionais")] public float? CustosAdicionais { get; set; }

    [SheetColumn("Id CTM")] public long? IdCTM { get; set; }

    [SheetColumn("Id Tipo Manutenção")] public long? IdTipoManutencao { get; set; }

    [SheetColumn("Id Ordem")] public long? IdOrdem { get; set; }

    [SheetColumn("Ferramentas")]
    public List<FerramentaTipoDto>? Ferramentas { get; set; } = new List<FerramentaTipoDto>();
}

#pragma warning restore CS1591