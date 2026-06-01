using app_pm_back.Attributes;
using System.Collections.Generic;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ordem de manutencao .
/// </summary>
public class OrdemManutencaoDto
{
    [SheetColumn("Id Ordem")] public long IdOrdem { get; set; }

    [SheetColumn("Ordem")] public long? NumeroOrdem { get; set; }

    [SheetColumn("Tipo de ordem")] public string? TipoOrdem { get; set; }

    [SheetColumn("Classificação")] public string? Classificacao { get; set; }

    [SheetColumn("Método classificação")] public string? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")] public string? ClassificadoPor { get; set; }

    [SheetColumn("Data classificação")] public DateTime? DataClassificacao { get; set; }

    [SheetColumn("Status sistema")] public string? StatusOrdem { get; set; }

    public string? Criticidade { get; set; }

    [SheetColumn("Prioridade")] public Prioridade? Prioridade { get; set; }

    [SheetColumn("Data de entrada")] public DateTime? DataCriacao { get; set; }

    [SheetColumn("Criado por")] public string? CriadaPorUsuario { get; set; }

    [SheetColumn("Modificado por")] public string? ModificadaPorUsuario { get; set; }

    [SheetColumn("Data modificacao")] public DateTime? DataModificacao { get; set; }

    [SheetColumn("Campo ordenação")] public string? DescricaoBreve { get; set; }

    [SheetColumn("Início prog.")] public DateTime? InicioPlanejado { get; set; }

    [SheetColumn("Fim programado")] public DateTime? FimPlanejado { get; set; }

    [SheetColumn("Liberação real")] public DateTime? InicioReal { get; set; }

    [SheetColumn("Dt.real fim")] public DateTime? FimReal { get; set; }

    [SheetColumn("Texto breve")] public ComponentType? IdAtivo { get; set; }

    [SheetColumn("Loc.instalação")] public string? IdLi { get; set; }

    [SheetColumn("Nota")] public long? IdNota { get; set; }

    public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }
}

#pragma warning restore CS1591