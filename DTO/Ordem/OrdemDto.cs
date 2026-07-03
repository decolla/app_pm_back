using app_pm_back.Attributes;
using System.Collections.Generic;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ordem de manutencao .
/// </summary>
public class OrdemDto
{

    [SheetColumn("numero_ordem")] public long? NumeroOrdem { get; set; }

    [SheetColumn("Tipo de ordem")] public string? TipoOrdem { get; set; }

    [SheetColumn("Classificação")] public Classificacao Classificacao { get; set; } = Classificacao.NAOCLASSIFICADA;

    [SheetColumn("Método classificação")]
    public MetodoClassificacao MetodoClassificacao { get; set; } = MetodoClassificacao.NAOCLASSIFICADA;

    [SheetColumn("Classificado por")] public string? ClassificadoPor { get; set; }

    [SheetColumn("Data classificação")] public DateTime? DataClassificacao { get; set; }

    [SheetColumn("status_ordem")] public StatusFichas? StatusOrdem { get; set; }

    public string? Criticidade { get; set; }

    [SheetColumn("Prioridade")] public Prioridade? Prioridade { get; set; }

    [SheetColumn("Data de entrada")] public DateTime? DataCriacao { get; set; }

    [SheetColumn("status_usuario")] public string? CriadaPorUsuario { get; set; }

    [SheetColumn("Modificado por")] public string? ModificadaPorUsuario { get; set; }

    [SheetColumn("Data modificacao")] public DateTime? DataModificacao { get; set; }

    [SheetColumn("Campo ordenação")] public string? DescricaoBreve { get; set; }

    [SheetColumn("Início prog.")] public DateTime? InicioPlanejado { get; set; }

    [SheetColumn("Fim programado")] public DateTime? FimPlanejado { get; set; }

    [SheetColumn("Liberação real")] public DateTime? InicioReal { get; set; }

    [SheetColumn("Dt.real fim")] public DateTime? FimReal { get; set; }

    [SheetColumn("Texto breve")] public ComponentType? IdAtivo { get; set; }

    [SheetColumn("cdequipamento")] public string? IdLi { get; set; }

    [SheetColumn("cdnotamanutencao")] public long? IdNota { get; set; }

    [SheetColumn("confiabilidade")]public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }

    [SheetColumn("Escopo")] public long? Escopo { get; set; }
    
    public List<OperacaoOrdemDto> Operacoes { get; set; } = new List<OperacaoOrdemDto>();
    
    public List<AnexoOrdemDto> Anexos { get; set; } = new List<AnexoOrdemDto>();
}

#pragma warning restore CS1591