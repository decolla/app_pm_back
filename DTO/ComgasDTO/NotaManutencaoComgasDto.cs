using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao de notas de manutencao Comgas.
/// </summary>
public class NotaManutencaoComgasDto
{
    [SheetColumn("Id Nota")]
    public long IdNota { get; set; }

    [SheetColumn("Nota")]
    public long NumeroNota { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoNota { get; set; }

    [SheetColumn("Classificação")]
    public string? Classificacao { get; set; }

    [SheetColumn("Método classificação")]
    public string? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")]
    public string? ClassificadoPor { get; set; }

    [SheetColumn("Data classificação")]
    public DateTime? DataClassificacao { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Texto")]
    public string? DescricaoDetalhada { get; set; }

    [SheetColumn("Status sistema")]
    public string? StatusNota { get; set; }

    [SheetColumn("Data da nota")]
    public DateTime DataNota { get; set; }

    [SheetColumn("Prioridade")]
    public int Prioridade { get; set; }

    [SheetColumn("Início desejado")]
    public DateTime? InicioDesejado { get; set; }

    [SheetColumn("Concl.desejada")]
    public DateTime? ConclusaoDesejada { get; set; }

    [SheetColumn("Equipamento")]
    public string? EquipamentoTag { get; set; }

    [SheetColumn("Loc.instalação")]
    public string? LocalInstalacaoCodigo { get; set; }

    [SheetColumn("Criado por")]
    public int? CriadoPorUsuarioId { get; set; }

    [SheetColumn("Tipo prioridade")]
    public string? TipoPrioridade { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoDeNota { get; set; }

    [SheetColumn("Txt.grp.codific")]
    public string? TxtGrpCodific { get; set; }

    [SheetColumn("Txt.code codif.")]
    public string? TxtCodeCodif { get; set; }

    [SheetColumn("Codificação")]
    public string? Codificacao { get; set; }

    [SheetColumn("Conseqüência")]
    public int? Consequencia { get; set; }

    [SheetColumn("Grupo de codes")]
    public string? GrupoDeCodes { get; set; }
}

#pragma warning restore CS1591
