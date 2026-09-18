using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de ferramenta vinculada ao tipo de manutencao.
/// </summary>
public class FerramentaTipoDto
{
    [SheetColumn("Id Ferramenta")] public long IdFerramenta { get; set; }

    [SheetColumn("Nome Ferramenta")] public string? Nome { get; set; }

    [SheetColumn("Categoria")] public string? Categoria { get; set; }

    [SheetColumn("Descrição Ferramenta")] public string? Descricao { get; set; }

    [SheetColumn("Tipo ferramenta")] public string? TipoFerramenta { get; set; }

    [SheetColumn("Quantidade")] public int Quantidade { get; set; }

    [SheetColumn("Loc.instalacao")] public string? IdLI { get; set; } = string.Empty;

    [SheetColumn("Fabricante")] public string? Fabricante { get; set; }

    [SheetColumn("Peso bruto")] public decimal? PesoBruto { get; set; }

    [SheetColumn("Unidade peso bruto")] public string? UmPesoBruto { get; set; }

    [SheetColumn("Peso liquido")] public decimal? PesoLiquido { get; set; }

    [SheetColumn("Unidade peso liquido")] public string? UmPesoLiquido { get; set; }

    [SheetColumn("Volume")] public decimal? Volume { get; set; }

    [SheetColumn("Unidade volume")] public string? UmVolume { get; set; }

    [SheetColumn("Dimensao")] public decimal? Dimensao { get; set; }

    [SheetColumn("Unidade dimensao")] public string? UmDimensao { get; set; }

    [SheetColumn("Valor")] public decimal? ValorCompra { get; set; }

    [SheetColumn("Data aquisição")] public DateTime? DataAquisicao { get; set; }

    // Escopo externo (apenas o ID).
    [SheetColumn("Id Escopo")] public long? IdEscopo { get; set; }

    [SheetColumn("Id Almoxarifado")] public long? IdAlmoxarifado { get; set; }
}

#pragma warning restore CS1591