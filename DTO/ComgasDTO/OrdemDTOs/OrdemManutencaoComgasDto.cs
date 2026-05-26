using app_pm_back.Attributes;
using System.Collections.Generic;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ordem de manutencao Comgas.
/// </summary>
public class OrdemManutencaoComgasDto
{
    [SheetColumn("Ordem")]
    public long IdOrdem { get; set; }

    [SheetColumn("Tipo de ordem")]
    public string? TipoOrdem { get; set; }

    [SheetColumn("Classificação")]
    public string? Classificacao { get; set; }

    [SheetColumn("Método classificação")]
    public string? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")]
    public string? ClassificadoPor { get; set; }

    [SheetColumn("Data classificação")]
    public DateTime? DataClassificacao { get; set; }

    [SheetColumn("Status sistema")]
    public string? StatusOrdem { get; set; }

    [SheetColumn("Nota")]
    public long? NotaRelacionadaId { get; set; }

    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoCodigo { get; set; }

    [SheetColumn("Prioridade")]
    public int? Prioridade { get; set; }

    [SheetColumn("Data de entrada")]
    public DateTime? DataEntrada { get; set; }

    [SheetColumn("Criado por")]
    public int? CriadaPorUsuarioId { get; set; }

    [SheetColumn("Modificado por")]
    public int? ModificadaPorUsuarioId { get; set; }

    [SheetColumn("Custs.estimados")]
    public decimal? CustoEstimado { get; set; }

    [SheetColumn("Custos tot.plan")]
    public decimal? CustoPlanejado { get; set; }

    [SheetColumn("Cust.tot.reais")]
    public decimal? CustoReal { get; set; }

    [SheetColumn("CenTrab respon.")]
    public string? CodigoTrabalho { get; set; }

    [SheetColumn("Denominação")]
    public string? Descricao { get; set; }
    
    [SheetColumn("Texto breve")]
    public string? DescricaoBreve { get; set; }

    [SheetColumn("Status usuário")]
    public string? Situacao { get; set; }

    [SheetColumn("Status paging")]
    public string? SituacaoPlanejamento { get; set; }

    [SheetColumn("Início prog.")]
    public DateTime? InicioPlanejado { get; set; }

    [SheetColumn("Fim programado")]
    public DateTime? FimPlanejado { get; set; }

    [SheetColumn("Liberação real")]
    public DateTime? InicioReal { get; set; }

    [SheetColumn("Dt.real fim")]
    public DateTime? FimReal { get; set; }

    [SheetColumn("Equipamento")]
    public string? IdAtivo { get; set; }

    [SheetColumn("Loc.instalação")]
    public int? IdLocalInstalacao { get; set; }

    [SheetColumn("Área operacion.")]
    public string? AreaOperacional { get; set; }

    [SheetColumn("Grp.plnj.PM")]
    public int? GpmResponsavelId { get; set; }

    [SheetColumn("Material")]
    public string? Material { get; set; }

    [SheetColumn("Nº de série")]
    public string? NumeroSerie { get; set; }

    [SheetColumn("Dds.dispositivo")]
    public string? DadosAdicionaisDispositivo { get; set; }

    [SheetColumn("Periodo")]
    public string? Periodo { get; set; }

    [SheetColumn("Nota")]
    public string? EtiquetaNota { get; set; }

    [SheetColumn("Data de entrada")]
    public DateTime? DataCriacao { get; set; }

    [SheetColumn("Analise criticidade")]
    public string? AnaliseCriticidade { get; set; }

    [SheetColumn("Analise risco")]
    public string? AnaliseRisco { get; set; }

    [SheetColumn("ARO/ADTC")]
    public string? AroAdtc { get; set; }

    [SheetColumn("Matriz LIBRA")]
    public string? MatrizLibra { get; set; }

    [SheetColumn("CenTrab respon.")]
    public string? PermissaoTrabalho { get; set; }

    public List<OperacaoOrdemComgasDto> Operacoes { get; set; } = new();
}

#pragma warning restore CS1591
