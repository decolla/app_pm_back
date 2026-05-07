using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de importacao de acoes de nota da Comgas.
/// </summary>
public class AcaoNotaCongasComgasDto
{
    [SheetColumn("Id Ação")]
    public int IdAcao { get; set; }

    [SheetColumn("Área operacion.")]
    public int? AreaOperac { get; set; }

    [SheetColumn("Atividade")]
    public string? Atividade { get; set; }

    [SheetColumn("TpCatálogo")]
    public string? TpCatalogo { get; set; }

    [SheetColumn("Bairro")]
    public string? Bairro { get; set; }

    [SheetColumn("Canal distrib.")]
    public string? CanalDistrib { get; set; }

    [SheetColumn("Centro custo")]
    public string? CentroCusto { get; set; }

    [SheetColumn("Cen.localiz.")]
    public string? CenLocaliz { get; set; }

    [SheetColumn("Cen.p/cen.trab.")]
    public string? CenPCenTrab { get; set; }

    [SheetColumn("Centro planej.")]
    public string? CentroPlanej { get; set; }

    [SheetColumn("CenTrab respon.")]
    public string? CenTrabRespon { get; set; }

    [SheetColumn("País")]
    public string? Pais { get; set; }

    [SheetColumn("Classe defeitos")]
    public string? ClasseDefeitos { get; set; }

    [SheetColumn("Cliente")]
    public string? Cliente { get; set; }

    [SheetColumn("Codificação")]
    public string? Codificacao { get; set; }

    [SheetColumn("Code parte obj.")]
    public string? CodeParteObj { get; set; }

    [SheetColumn("Código de ação")]
    public string? CodigoDeAcao { get; set; }

    [SheetColumn("Código do dano")]
    public string? CodigoDoDano { get; set; }

    [SheetColumn("Concl.desejada")]
    public DateTime? ConclDesejada { get; set; }

    [SheetColumn("Data da nota")]
    public DateTime? DataDaNota { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Equipamento")]
    public string? Equipamento { get; set; }

    [SheetColumn("Loc.instalação")]
    public string? LocInstalacao { get; set; }

    [SheetColumn("Nota")]
    public string? Nota { get; set; }

    [SheetColumn("Ordem")]
    public string? Ordem { get; set; }

    [SheetColumn("Texto")]
    public string? Texto { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoDeNota { get; set; }

    [SheetColumn("Tipo prioridade")]
    public string? TipoPrioridade { get; set; }
}

